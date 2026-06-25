using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HKTech.Data;
using HKTech.Models;
using HKTech.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HKTech.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductAdminController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment  _env;

    public ProductAdminController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db  = db;
        _env = env;
    }

    // ── Index ─────────────────────────────────────────────────────────────
    public async Task<IActionResult> Index(string? category, string? search, int page = 1)
    {
        const int pageSize = 15;
        var query = _db.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .AsQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(p => p.Category.Slug == category);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Name.Contains(search));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.TotalCount  = total;
        ViewBag.Page        = page;
        ViewBag.PageSize    = pageSize;
        ViewBag.TotalPages  = (int)Math.Ceiling(total / (double)pageSize);
        ViewBag.Search      = search;
        ViewBag.Category    = category;
        ViewBag.Categories  = await _db.Categories.ToListAsync();
        return View(items);
    }

    // ── Create ────────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateCategoriesAsync();
        return View(new ProductFormViewModel { IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync();
            return View(vm);
        }

        var product = new Product
        {
            Name           = vm.Name,
            Description    = vm.Description,
            Price          = vm.Price,
            CategoryId     = vm.CategoryId,
            TdpWatt        = vm.TdpWatt,
            BenchmarkScore = vm.BenchmarkScore,
            Socket         = vm.Socket,
            RamType        = vm.RamType,
            FormFactor     = vm.FormFactor,
            StockQuantity  = vm.StockQuantity,
            IsActive       = vm.IsActive,
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        await SaveImagesAsync(product.Id, vm.ImageFiles, isPrimaryFirst: true);

        TempData["Success"] = $"✅ Đã thêm sản phẩm \"{product.Name}\"";
        return RedirectToAction(nameof(Index));
    }

    // ── Edit ──────────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _db.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null) return NotFound();

        await PopulateCategoriesAsync();
        return View(new ProductFormViewModel
        {
            Id             = product.Id,
            Name           = product.Name,
            Description    = product.Description,
            Price          = product.Price,
            CategoryId     = product.CategoryId,
            TdpWatt        = product.TdpWatt,
            BenchmarkScore = product.BenchmarkScore,
            Socket         = product.Socket,
            RamType        = product.RamType,
            FormFactor     = product.FormFactor,
            StockQuantity  = product.StockQuantity,
            IsActive       = product.IsActive,
            ExistingImages = product.Images.ToList(),
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync();
            vm.ExistingImages = await _db.ProductImages
                .Where(i => i.ProductId == id).ToListAsync();
            return View(vm);
        }

        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();

        product.Name           = vm.Name;
        product.Description    = vm.Description;
        product.Price          = vm.Price;
        product.CategoryId     = vm.CategoryId;
        product.TdpWatt        = vm.TdpWatt;
        product.BenchmarkScore = vm.BenchmarkScore;
        product.Socket         = vm.Socket;
        product.RamType        = vm.RamType;
        product.FormFactor     = vm.FormFactor;
        product.StockQuantity  = vm.StockQuantity;
        product.IsActive       = vm.IsActive;

        // Xóa ảnh được chọn
        if (vm.DeleteImageIds.Any())
        {
            var toDelete = await _db.ProductImages
                .Where(i => vm.DeleteImageIds.Contains(i.Id)).ToListAsync();
            foreach (var img in toDelete)
                DeleteImageFile(img.ImageUrl);
            _db.ProductImages.RemoveRange(toDelete);
        }

        await _db.SaveChangesAsync();
        await SaveImagesAsync(product.Id, vm.ImageFiles, isPrimaryFirst: false);

        TempData["Success"] = $"✅ Đã cập nhật sản phẩm \"{product.Name}\"";
        return RedirectToAction(nameof(Index));
    }

    // ── Delete ────────────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null) return NotFound();

        foreach (var img in product.Images)
            DeleteImageFile(img.ImageUrl);

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"🗑️ Đã xóa sản phẩm \"{product.Name}\"";
        return RedirectToAction(nameof(Index));
    }

    // ── Toggle IsActive ───────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();
        product.IsActive = !product.IsActive;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // ── Helpers ───────────────────────────────────────────────────────────
    private async Task PopulateCategoriesAsync()
    {
        var cats = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        ViewBag.Categories = new SelectList(cats, "Id", "Name");
    }

    private async Task SaveImagesAsync(int productId, List<IFormFile>? files, bool isPrimaryFirst)
    {
        if (files is null || !files.Any()) return;

        var uploadDir = Path.Combine(_env.WebRootPath, "images", "products");
        Directory.CreateDirectory(uploadDir);

        bool firstImage = isPrimaryFirst && !await _db.ProductImages.AnyAsync(i => i.ProductId == productId);

        foreach (var file in files.Where(f => f.Length > 0))
        {
            var ext      = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{productId}_{Guid.NewGuid():N}{ext}";
            var filePath = Path.Combine(uploadDir, fileName);

            using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);

            _db.ProductImages.Add(new ProductImage
            {
                ProductId = productId,
                ImageUrl  = $"/images/products/{fileName}",
                IsPrimary = firstImage,
            });
            firstImage = false;
        }
        await _db.SaveChangesAsync();
    }

    private void DeleteImageFile(string imageUrl)
    {
        try
        {
            var path = Path.Combine(_env.WebRootPath, imageUrl.TrimStart('/'));
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }
        catch { /* ignore */ }
    }
}
