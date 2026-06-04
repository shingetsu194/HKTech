using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HKTech.Data;
using HKTech.Models;
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
        // ── Stat cards ────────────────────────────────────────────────────
        ViewBag.TotalProducts   = await _db.Products.CountAsync(p => p.IsActive);
        ViewBag.TotalCategories = await _db.Categories.CountAsync();
        ViewBag.TotalOrders     = await _db.Orders.CountAsync();
        ViewBag.TotalUsers      = await _db.Users.CountAsync();

        // ── Doanh thu ─────────────────────────────────────────────────────
        ViewBag.TotalRevenue = await _db.Orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

        // ── Đơn hàng theo trạng thái ──────────────────────────────────────
        ViewBag.CountPending   = await _db.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
        ViewBag.CountConfirmed = await _db.Orders.CountAsync(o => o.Status == OrderStatus.Confirmed);
        ViewBag.CountShipping  = await _db.Orders.CountAsync(o => o.Status == OrderStatus.Shipping);
        ViewBag.CountDelivered = await _db.Orders.CountAsync(o => o.Status == OrderStatus.Delivered);
        ViewBag.CountCancelled = await _db.Orders.CountAsync(o => o.Status == OrderStatus.Cancelled);

        // ── Sản phẩm sắp hết hàng (≤ 5) ──────────────────────────────────
        ViewBag.LowStock = await _db.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive && p.StockQuantity <= 5)
            .OrderBy(p => p.StockQuantity)
            .Take(8)
            .ToListAsync();

        // ── 10 đơn hàng gần nhất ──────────────────────────────────────────
        ViewBag.RecentOrders = await _db.Orders
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
            .OrderByDescending(o => o.CreatedAt)
            .Take(10)
            .ToListAsync();

        return View();
    }
}
