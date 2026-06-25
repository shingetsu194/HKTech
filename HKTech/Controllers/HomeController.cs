using System.Diagnostics;
using HKTech.Data;
using HKTech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HKTech.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        // Linh kiện nổi bật — CHỈ lấy linh kiện, loại bỏ PC build sẵn
        var featured = await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.IsActive && p.Category.Slug != "prebuild")
            .OrderByDescending(p => p.Id)
            .Take(8)
            .ToListAsync();

        // PC build sẵn nổi bật — hiển thị ở section riêng
        var prebuilts = await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.IsActive && p.Category.Slug == "prebuild")
            .OrderByDescending(p => p.Id)
            .Take(8)
            .ToListAsync();

        ViewBag.Prebuilts = prebuilts;
        return View(featured);
    }

    public IActionResult Privacy() => View();

    [Route("/Home/Error404")]
    public IActionResult Error404() => View("~/Views/Shared/Error404.cshtml");

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
