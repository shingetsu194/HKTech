using HKTech.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HKTech.Controllers;

/// <summary>
/// Controller cho trang danh sách linh kiện — Phase 2: bind từ DB.
/// </summary>
public class ProductController : Controller
{
    private readonly ApplicationDbContext _db;
    public ProductController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index(string? category, string? search, string? sort,
                                           decimal? priceMin, decimal? priceMax, int page = 1)
    {
        const int pageSize = 12;

        var query = _db.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.IsActive && p.Category.Slug != "prebuild")
            .AsQueryable();

        // Filter
        if (!string.IsNullOrEmpty(category))
            query = query.Where(p => p.Category.Slug == category);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Name.Contains(search) || (p.Description != null && p.Description.Contains(search)));

        if (priceMin.HasValue) query = query.Where(p => p.Price >= priceMin);
        if (priceMax.HasValue) query = query.Where(p => p.Price <= priceMax);

        // Sort
        query = sort switch
        {
            "price-asc"  => query.OrderBy(p => p.Price),
            "price-desc" => query.OrderByDescending(p => p.Price),
            "name-asc"   => query.OrderBy(p => p.Name),
            _            => query.OrderByDescending(p => p.Id),
        };

        var totalCount = await query.CountAsync();
        var products   = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Categories  = await _db.Categories.ToListAsync();
        ViewBag.Category    = category;
        ViewBag.Search      = search;
        ViewBag.Sort        = sort;
        ViewBag.PriceMin    = priceMin;
        ViewBag.PriceMax    = priceMax;
        ViewBag.TotalCount  = totalCount;
        ViewBag.Page        = page;
        ViewBag.PageSize    = pageSize;
        ViewBag.TotalPages  = (int)Math.Ceiling(totalCount / (double)pageSize);

        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var product = await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        if (product is null) return NotFound();

        // Related products cùng danh mục
        ViewBag.Related = await _db.Products
            .Include(p => p.Images)
            .Where(p => p.CategoryId == product.CategoryId && p.Id != id && p.IsActive)
            .Take(4)
            .ToListAsync();

        return View(product);
    }
}
