// ================================================================
// HKTECH — RecommendController.cs
// Phase 3: Thuật toán gợi ý combo linh kiện theo budget & useCase
// ================================================================

using HKTech.Data;
using HKTech.Models;
using HKTech.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HKTech.Controllers
{
    public class RecommendController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly CartService _cart;

        public RecommendController(ApplicationDbContext db, CartService cart)
        {
            _db  = db;
            _cart = cart;
        }

        // ================================================================
        // GET /Recommend
        // ================================================================
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "Gợi ý cấu hình PC";
            return View();
        }

        // ================================================================
        // POST /Recommend — Thuật toán query combo
        // ================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            decimal budget, string useCase, int ramPref = 16, string? notes = null)
        {
            ViewData["Title"] = "Gợi ý cấu hình PC";
            ViewBag.Budget    = budget;
            ViewBag.UseCase   = useCase;
            ViewBag.RamPref   = ramPref;

            if (budget <= 0)
            {
                ViewBag.Error = "Vui lòng chọn ngân sách hợp lệ.";
                return View();
            }

            // ── Phân bổ ngân sách theo useCase ───────────────────────────
            // Mỗi slot được % ngân sách khác nhau tùy mục đích
            var alloc = GetBudgetAllocation(useCase, budget);

            // ── Query sản phẩm tốt nhất theo từng slot ───────────────────
            var products = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.IsActive)
                .ToListAsync();

            // Hàm tìm sản phẩm tốt nhất trong slot: BenchmarkScore cao nhất,
            // không vượt budget slot, RAM type phải khớp nếu có
            Product? Pick(string slug, decimal slotBudget,
                          string? ramType = null, string? socket = null)
            {
                var candidates = products
                    .Where(p => p.Category.Slug == slug
                             && p.Price <= slotBudget
                             && p.Price > 0);

                // Filter RAM type nếu cần
                if (ramType != null)
                    candidates = candidates.Where(p =>
                        p.RamType == null || p.RamType == ramType);

                // Filter socket nếu cần
                if (socket != null)
                    candidates = candidates.Where(p =>
                        p.Socket == null || p.Socket == socket);

                return candidates
                    .OrderByDescending(p => p.BenchmarkScore)
                    .ThenByDescending(p => p.Price)
                    .FirstOrDefault();
            }

            // ── Build 3 combo: Budget / Balanced / Premium ────────────────
            var combos = new List<RecommendCombo>();

            foreach (var (label, multiplier, badge) in new[]
            {
                ("Tiết kiệm",   0.75m, "BUDGET"),
                ("Cân bằng",    1.00m, "BALANCED"),
                ("Hiệu năng cao", 1.25m, "PERFORMANCE"),
            })
            {
                // Giới hạn combo premium không vượt budget × 1.25
                if (multiplier > 1.0m && budget * multiplier > budget * 1.3m)
                    continue;

                var scaledAlloc = ScaleBudget(alloc, multiplier);
                var cpu = Pick("cpu", scaledAlloc["cpu"]);
                if (cpu == null) continue;   // không có CPU → bỏ combo này

                // Mainboard phải cùng socket với CPU
                var mb  = Pick("mainboard", scaledAlloc["mainboard"], socket: cpu.Socket);
                var ram = Pick("ram",       scaledAlloc["ram"],
                               ramType: mb?.RamType ?? (useCase == "workstation" ? "DDR5" : null));
                var gpu     = Pick("gpu",     scaledAlloc["gpu"]);
                var storage = Pick("storage", scaledAlloc["storage"]);
                var psu     = Pick("psu",     scaledAlloc["psu"]);
                var pcCase  = Pick("case",    scaledAlloc["case"]);
                var cooling = Pick("cooling", scaledAlloc["cooling"]);

                // Đảm bảo cấu hình luôn có đủ bo mạch chủ, RAM và nguồn
                if (mb == null || ram == null || psu == null) continue;

                // Gaming/Esports bắt buộc phải có GPU
                if ((useCase == "gaming" || useCase == "esports") && gpu == null) continue;

                var items = new[] { cpu, mb, ram, gpu, storage, psu, pcCase, cooling }
                    .Where(p => p != null).Cast<Product>().ToList();

                if (items.Count < 4) continue; // quá ít linh kiện → bỏ

                var total = items.Sum(p => p.Price);
                var tdp   = items.Sum(p => p.TdpWatt);

                // Tính bottleneck CPU↔GPU
                int bottleneck = 0;
                if (cpu != null && gpu != null && cpu.BenchmarkScore > 0 && gpu.BenchmarkScore > 0)
                {
                    var diff = Math.Abs(cpu.BenchmarkScore - gpu.BenchmarkScore);
                    var max  = Math.Max(cpu.BenchmarkScore, gpu.BenchmarkScore);
                    bottleneck = (int)(diff * 100.0 / max);
                }

                combos.Add(new RecommendCombo
                {
                    Label      = label,
                    Badge      = badge,
                    Items      = items,
                    TotalPrice = total,
                    TotalTdp   = tdp,
                    Bottleneck = bottleneck,
                    UseCase    = useCase,
                });
            }

            if (!combos.Any())
            {
                ViewBag.Error = "Ngân sách chưa đủ để gợi ý cấu hình. Hãy thử tăng ngân sách.";
                return View();
            }

            ViewBag.Combos = combos;
            return View();
        }

        // ================================================================
        // POST /Recommend/AddComboToCart — Thêm combo vào giỏ hàng
        // ================================================================
        [HttpPost]
        public async Task<IActionResult> AddComboToCart([FromBody] List<int> productIds)
        {
            if (productIds == null || !productIds.Any())
                return Json(new { success = false, message = "Không có sản phẩm." });

            var added = await _cart.AddBuildToCartAsync(productIds);
            return Json(new
            {
                success   = true,
                message   = $"Đã thêm {added} linh kiện vào giỏ hàng!",
                cartCount = _cart.GetItemCount(),
            });
        }

        // ================================================================
        // HELPERS — Phân bổ ngân sách theo useCase
        // ================================================================
        private static Dictionary<string, decimal> GetBudgetAllocation(
            string useCase, decimal budget)
        {
            // % phân bổ cho từng slot tùy mục đích sử dụng
            var pct = useCase switch
            {
                "gaming" => new Dictionary<string, float>
                {
                    ["cpu"]      = 0.20f,
                    ["mainboard"]= 0.10f,
                    ["ram"]      = 0.07f,
                    ["gpu"]      = 0.38f,   // GPU ưu tiên nhất
                    ["storage"]  = 0.07f,
                    ["psu"]      = 0.08f,
                    ["case"]     = 0.05f,
                    ["cooling"]  = 0.05f,
                },
                "esports" => new Dictionary<string, float>
                {
                    ["cpu"]      = 0.28f,   // CPU cao để FPS cao
                    ["mainboard"]= 0.10f,
                    ["ram"]      = 0.08f,
                    ["gpu"]      = 0.28f,
                    ["storage"]  = 0.06f,
                    ["psu"]      = 0.08f,
                    ["case"]     = 0.06f,
                    ["cooling"]  = 0.06f,
                },
                "workstation" => new Dictionary<string, float>
                {
                    ["cpu"]      = 0.32f,   // CPU + RAM ưu tiên
                    ["mainboard"]= 0.12f,
                    ["ram"]      = 0.18f,
                    ["gpu"]      = 0.18f,
                    ["storage"]  = 0.08f,
                    ["psu"]      = 0.06f,
                    ["case"]     = 0.04f,
                    ["cooling"]  = 0.02f,
                },
                "streaming" => new Dictionary<string, float>
                {
                    ["cpu"]      = 0.28f,   // CPU quan trọng để encode
                    ["mainboard"]= 0.10f,
                    ["ram"]      = 0.10f,
                    ["gpu"]      = 0.25f,
                    ["storage"]  = 0.10f,   // SSD nhiều để lưu footage
                    ["psu"]      = 0.07f,
                    ["case"]     = 0.06f,
                    ["cooling"]  = 0.04f,
                },
                "office" => new Dictionary<string, float>
                {
                    ["cpu"]      = 0.25f,
                    ["mainboard"]= 0.12f,
                    ["ram"]      = 0.12f,
                    ["gpu"]      = 0.12f,
                    ["storage"]  = 0.12f,
                    ["psu"]      = 0.10f,
                    ["case"]     = 0.10f,
                    ["cooling"]  = 0.07f,
                },
                _ => new Dictionary<string, float>   // balance
                {
                    ["cpu"]      = 0.22f,
                    ["mainboard"]= 0.11f,
                    ["ram"]      = 0.09f,
                    ["gpu"]      = 0.30f,
                    ["storage"]  = 0.08f,
                    ["psu"]      = 0.08f,
                    ["case"]     = 0.07f,
                    ["cooling"]  = 0.05f,
                },
            };

            return pct.ToDictionary(
                kvp => kvp.Key,
                kvp => Math.Round(budget * (decimal)kvp.Value, -3)); // làm tròn 1000đ
        }

        private static Dictionary<string, decimal> ScaleBudget(
            Dictionary<string, decimal> alloc, decimal multiplier)
            => alloc.ToDictionary(kvp => kvp.Key, kvp => kvp.Value * multiplier);
    }

    // ================================================================
    // ViewModel: 1 combo gợi ý hoàn chỉnh
    // ================================================================
    public class RecommendCombo
    {
        public string  Label      { get; set; } = string.Empty;
        public string  Badge      { get; set; } = string.Empty;
        public List<Product> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public int     TotalTdp   { get; set; }
        public int     Bottleneck { get; set; }  // % bottleneck CPU↔GPU
        public string  UseCase    { get; set; } = string.Empty;

        // Badge màu theo label
        public string BadgeColor => Badge switch
        {
            "BUDGET"      => "#44CC44",
            "BALANCED"    => "#FFD700",
            "PERFORMANCE" => "#FF4444",
            _             => "#AAAAAA",
        };

        public IEnumerable<int> ProductIds => Items.Select(p => p.Id);
    }
}
