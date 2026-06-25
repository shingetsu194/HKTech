// ================================================================
// HKTECH — PrebuiltController.cs
// Trang PC Build Sẵn: gallery + filter theo source / giá / GPU
// ================================================================

using HKTech.Data;
using HKTech.Models;
using HKTech.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HKTech.Controllers
{
    public class PrebuiltController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly CartService _cart;

        public PrebuiltController(ApplicationDbContext db, CartService cart)
        {
            _db   = db;
            _cart = cart;
        }

        // ── GET /Prebuilt ──────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Index(
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? gpu = null,
            string? sort = null)
        {
            ViewData["Title"] = "PC Build Sẵn";

            var query = _db.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.IsActive && p.Category.Slug == "prebuild");

            // ── Filters ──────────────────────────────────────────────────
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            if (!string.IsNullOrEmpty(gpu))
                query = query.Where(p => p.Description != null && p.Description.ToLower().Contains(gpu.ToLower()));

            // ── Sort ─────────────────────────────────────────────────────
            query = sort switch
            {
                "price-asc"  => query.OrderBy(p => p.Price),
                "price-desc" => query.OrderByDescending(p => p.Price),
                "name-asc"   => query.OrderBy(p => p.Name),
                _            => query.OrderBy(p => p.Name)
            };

            var prebuilts = await query.ToListAsync();

            ViewBag.SelMinPrice = minPrice;
            ViewBag.SelMaxPrice = maxPrice;
            ViewBag.SelGpu      = gpu;
            ViewBag.SelSort     = sort;

            return View(prebuilts);
        }

        // ── GET /Prebuilt/Details/{id} ─────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var pc = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id
                                       && p.IsActive
                                       && p.Category.Slug == "prebuild");

            if (pc == null) return NotFound();

            // ── Ước tính FPS từ tên CPU + GPU trong description ─────────────
            var specs = (pc.Description ?? "").Split('|')
                .Select(s => s.Split(':', 2))
                .Where(a => a.Length == 2)
                .ToDictionary(a => a[0].Trim(), a => a[1].Trim());

            string cpuName = specs.GetValueOrDefault("fullCpu") ?? specs.GetValueOrDefault("cpu") ?? "";
            string gpuName = specs.GetValueOrDefault("fullGpu") ?? specs.GetValueOrDefault("gpu") ?? "";

            int cpuScore = HardwareSpecEstimator.Estimate("cpu", cpuName)?.BenchmarkScore ?? 0;
            int gpuScore = HardwareSpecEstimator.Estimate("gpu", gpuName)?.BenchmarkScore ?? 0;

            ViewBag.CpuScore = cpuScore;
            ViewBag.GpuScore = gpuScore;
            ViewBag.FpsTable = FpsCalculator.Calc(cpuScore, gpuScore);
            ViewBag.Badge    = FpsCalculator.Badge(cpuScore, gpuScore);

            ViewData["Title"] = pc.Name;
            return View(pc);
        }

        // ── POST /Prebuilt/AddToCart ───────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });

            await _cart.AddItemAsync(id, 1);
            return Json(new
            {
                success   = true,
                message   = $"Đã thêm \"{product.Name}\" vào giỏ hàng!",
                cartCount = _cart.GetItemCount(),
            });
        }
    }
}
