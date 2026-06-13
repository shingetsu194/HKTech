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
            string? source = null,
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
            if (!string.IsNullOrEmpty(source))
                query = query.Where(p => p.Description != null && p.Description.Contains($"source:{source}"));

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

            // ── ViewBag data ─────────────────────────────────────────────
            // Collect distinct sources from DB
            var allSources = await _db.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.Category.Slug == "prebuild" && p.Description != null)
                .Select(p => p.Description!)
                .ToListAsync();

            var sources = allSources
                .SelectMany(d => d.Split('|'))
                .Where(s => s.StartsWith("source:"))
                .Select(s => s.Replace("source:", "").Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            ViewBag.Sources     = sources;
            ViewBag.SelSource   = source;
            ViewBag.SelMinPrice = minPrice;
            ViewBag.SelMaxPrice = maxPrice;
            ViewBag.SelGpu      = gpu;
            ViewBag.SelSort     = sort;

            return View(prebuilts);
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
