// ================================================================
// HKTECH — BuildController.cs
// Phase 3: Kết nối DB, Session-based build, lưu build cho user
// ================================================================

using HKTech.Data;
using HKTech.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HKTech.Controllers
{
    public class BuildController : Controller
    {
        // Session key lưu build tạm thời
        private const string SESSION_BUILD_KEY = "CurrentBuild";

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public BuildController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ── Mapping: slot key → category slug ────────────────────────────
        private static readonly Dictionary<string, string> SlotToCategory = new()
        {
            { "cpu",      "cpu"       },
            { "mainboard","mainboard" },
            { "ram",      "ram"       },
            { "gpu",      "gpu"       },
            { "storage",  "storage"   },
            { "psu",      "psu"       },
            { "case",     "case"      },
            { "cooling",  "cooling"   },
        };

        // ================================================================
        // GET /Build — Trang chính Build PC
        // ================================================================
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Tự Build PC";

            // Load danh sách sản phẩm theo từng slot từ DB
            var allProducts = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.IsActive)
                .ToListAsync();

            // Gom theo category slug
            var productsBySlot = SlotToCategory.ToDictionary(
                kvp => kvp.Key,
                kvp => allProducts
                    .Where(p => p.Category.Slug == kvp.Value)
                    .ToList()
            );

            // Load build hiện tại từ Session
            var currentBuild = GetBuildFromSession();

            ViewBag.ProductsBySlot = productsBySlot;
            ViewBag.CurrentBuild   = currentBuild;

            return View();
        }

        // ================================================================
        // POST /Build/SelectProduct — Chọn sản phẩm vào slot
        // ================================================================
        [HttpPost]
        public async Task<IActionResult> SelectProduct(string slot, int productId)
        {
            if (!SlotToCategory.ContainsKey(slot))
                return Json(new { success = false, message = "Slot không hợp lệ." });

            var product = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);

            if (product == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });

            // Cập nhật Session build
            var build = GetBuildFromSession();
            build[slot] = new BuildSlotItem
            {
                ProductId    = product.Id,
                Name         = product.Name,
                Price        = product.Price,
                ImageUrl     = product.PrimaryImageUrl ?? "/images/placeholder.png",
                TdpWatt      = product.TdpWatt,
                BenchmarkScore = product.BenchmarkScore,
                Socket       = product.Socket,
                RamType      = product.RamType,
                FormFactor   = product.FormFactor,
                CategorySlug = product.Category.Slug,
            };
            SaveBuildToSession(build);

            var totalPrice = build.Values.Sum(x => x.Price);
            var filledCount = build.Count;

            return Json(new
            {
                success      = true,
                slot,
                productId    = product.Id,
                name         = product.Name,
                price        = product.Price,
                priceFormatted = product.Price.ToString("N0") + " ₫",
                imageUrl     = product.PrimaryImageUrl ?? "/images/placeholder.png",
                totalPrice,
                totalPriceFormatted = totalPrice.ToString("N0") + " ₫",
                filledCount,
            });
        }

        // ================================================================
        // POST /Build/RemoveSlot — Xóa linh kiện khỏi slot
        // ================================================================
        [HttpPost]
        public IActionResult RemoveSlot(string slot)
        {
            var build = GetBuildFromSession();
            build.Remove(slot);
            SaveBuildToSession(build);

            var totalPrice = build.Values.Sum(x => x.Price);

            return Json(new
            {
                success = true,
                slot,
                totalPrice,
                totalPriceFormatted = totalPrice.ToString("N0") + " ₫",
                filledCount = build.Count,
            });
        }

        // ================================================================
        // POST /Build/ClearAll — Xóa toàn bộ build
        // ================================================================
        [HttpPost]
        public IActionResult ClearAll()
        {
            HttpContext.Session.Remove(SESSION_BUILD_KEY);
            return Json(new { success = true });
        }

        // ================================================================
        // POST /Build/SaveBuild — Lưu build vào DB (yêu cầu đăng nhập)
        // ================================================================
        [HttpPost]
        public async Task<IActionResult> SaveBuild(string buildName = "My Build")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Vui lòng đăng nhập để lưu build." });

            var build = GetBuildFromSession();
            if (!build.Any())
                return Json(new { success = false, message = "Build đang trống." });

            var pcBuild = new PcBuild
            {
                UserId     = user.Id,
                Name       = string.IsNullOrWhiteSpace(buildName) ? "My Build" : buildName,
                TotalPrice = build.Values.Sum(x => x.Price),
                CreatedAt  = DateTime.UtcNow,
                Items      = build.Select(kvp => new PcBuildItem
                {
                    ProductId = kvp.Value.ProductId,
                    Slot      = kvp.Key,
                }).ToList()
            };

            _db.PcBuilds.Add(pcBuild);
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = $"Đã lưu build \"{pcBuild.Name}\"!", buildId = pcBuild.Id });
        }

        // ================================================================
        // GET /Build/GetProducts?slot=cpu — API lấy sản phẩm theo slot
        // ================================================================
        [HttpGet]
        public async Task<IActionResult> GetProducts(string slot)
        {
            if (!SlotToCategory.TryGetValue(slot, out var categorySlug))
                return Json(new { success = false });

            var products = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.Category.Slug == categorySlug && p.IsActive)
                .OrderBy(p => p.Price)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    priceFormatted = p.Price.ToString("N0") + " ₫",
                    p.TdpWatt,
                    p.BenchmarkScore,
                    p.Socket,
                    p.RamType,
                    p.FormFactor,
                    imageUrl = p.Images.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault()
                              ?? p.Images.Select(i => i.ImageUrl).FirstOrDefault()
                              ?? "/images/placeholder.png",
                })
                .ToListAsync();

            return Json(new { success = true, products });
        }

        // ================================================================
        // HELPERS — Session serialization
        // ================================================================
        private Dictionary<string, BuildSlotItem> GetBuildFromSession()
        {
            var json = HttpContext.Session.GetString(SESSION_BUILD_KEY);
            if (string.IsNullOrEmpty(json))
                return new Dictionary<string, BuildSlotItem>();
            return JsonSerializer.Deserialize<Dictionary<string, BuildSlotItem>>(json)
                   ?? new Dictionary<string, BuildSlotItem>();
        }

        private void SaveBuildToSession(Dictionary<string, BuildSlotItem> build)
        {
            HttpContext.Session.SetString(SESSION_BUILD_KEY,
                JsonSerializer.Serialize(build));
        }
    }

    // ================================================================
    // ViewModel: thông tin 1 slot trong session build
    // ================================================================
    public class BuildSlotItem
    {
        public int     ProductId      { get; set; }
        public string  Name           { get; set; } = string.Empty;
        public decimal Price          { get; set; }
        public string  ImageUrl       { get; set; } = string.Empty;
        public int     TdpWatt        { get; set; }
        public int     BenchmarkScore { get; set; }
        public string? Socket         { get; set; }
        public string? RamType        { get; set; }
        public string? FormFactor     { get; set; }
        public string  CategorySlug   { get; set; } = string.Empty;
    }
}
