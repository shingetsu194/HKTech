using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HKTech.Data;
using Microsoft.EntityFrameworkCore;

namespace HKTech.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;

    public DashboardController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalProducts   = await _db.Products.CountAsync();
        ViewBag.TotalCategories = await _db.Categories.CountAsync();
        ViewBag.TotalOrders     = await _db.Orders.CountAsync();
        ViewBag.TotalUsers      = await _db.Users.CountAsync();
        ViewBag.RecentOrders    = await _db.Orders
            .Include(o => o.User)
            .OrderByDescending(o => o.CreatedAt)
            .Take(5)
            .ToListAsync();
        return View();
    }
}
