using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HKTech.Data;
using HKTech.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HKTech.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoryAdminController : Controller
{
    private readonly ApplicationDbContext _db;
    public CategoryAdminController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
        => View(await _db.Categories
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .ToListAsync());

    [HttpGet]
    public IActionResult Create() => View(new Category());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category vm)
    {
        if (!ModelState.IsValid) return View(vm);

        // Auto-generate slug nếu bỏ trống
        if (string.IsNullOrWhiteSpace(vm.Slug))
            vm.Slug = GenerateSlug(vm.Name);

        if (await _db.Categories.AnyAsync(c => c.Slug == vm.Slug))
        {
            ModelState.AddModelError("Slug", "Slug này đã tồn tại");
            return View(vm);
        }

        _db.Categories.Add(vm);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"✅ Đã thêm danh mục \"{vm.Name}\"";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var cat = await _db.Categories.FindAsync(id);
        return cat is null ? NotFound() : View(cat);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var cat = await _db.Categories.FindAsync(id);
        if (cat is null) return NotFound();

        cat.Name        = vm.Name;
        cat.Slug        = vm.Slug;
        cat.Icon        = vm.Icon;
        cat.Description = vm.Description;

        await _db.SaveChangesAsync();
        TempData["Success"] = $"✅ Đã cập nhật danh mục \"{cat.Name}\"";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var cat = await _db.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cat is null) return NotFound();

        if (cat.Products.Any())
        {
            TempData["Error"] = "❌ Không thể xóa danh mục đang có sản phẩm!";
            return RedirectToAction(nameof(Index));
        }

        _db.Categories.Remove(cat);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"🗑️ Đã xóa danh mục \"{cat.Name}\"";
        return RedirectToAction(nameof(Index));
    }

    private static string GenerateSlug(string name)
        => name.ToLower()
               .Replace(" ", "-")
               .Replace("đ", "d");
}
