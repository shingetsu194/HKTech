using HKTech.Data;
using HKTech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HKTech.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class OrderAdminController : Controller
{
    private readonly ApplicationDbContext _db;
    public OrderAdminController(ApplicationDbContext db) => _db = db;

    // ── Index — danh sách đơn hàng ────────────────────────────────────────
    public async Task<IActionResult> Index(string? status, string? search, int page = 1)
    {
        const int pageSize = 15;

        var query = _db.Orders
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(o => o.Status == status);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(o =>
                o.User!.FullName!.Contains(search) ||
                o.User!.Email!.Contains(search) ||
                o.Id.ToString() == search);

        var total = await query.CountAsync();
        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Status     = status;
        ViewBag.Search     = search;
        ViewBag.Page       = page;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        ViewBag.Total      = total;

        // Đếm theo từng trạng thái cho filter bar
        ViewBag.CountPending   = await _db.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
        ViewBag.CountConfirmed = await _db.Orders.CountAsync(o => o.Status == OrderStatus.Confirmed);
        ViewBag.CountShipping  = await _db.Orders.CountAsync(o => o.Status == OrderStatus.Shipping);
        ViewBag.CountDelivered = await _db.Orders.CountAsync(o => o.Status == OrderStatus.Delivered);
        ViewBag.CountCancelled = await _db.Orders.CountAsync(o => o.Status == OrderStatus.Cancelled);

        return View(orders);
    }

    // ── Detail — chi tiết đơn hàng ───────────────────────────────────────
    public async Task<IActionResult> Detail(int id)
    {
        var order = await _db.Orders
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
                    .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null) return NotFound();

        // Các trạng thái có thể chuyển tiếp từ trạng thái hiện tại
        ViewBag.NextStatuses = order.Status switch
        {
            OrderStatus.Pending   => new[] { OrderStatus.Confirmed, OrderStatus.Cancelled },
            OrderStatus.Confirmed => new[] { OrderStatus.Shipping,  OrderStatus.Cancelled },
            OrderStatus.Shipping  => new[] { OrderStatus.Delivered, OrderStatus.Cancelled },
            _                     => Array.Empty<string>(),
        };

        return View(order);
    }

    // ── UpdateStatus — cập nhật trạng thái ──────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string newStatus)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order is null) return NotFound();

        // Validate chuyển trạng thái hợp lệ
        var allowed = order.Status switch
        {
            OrderStatus.Pending   => new[] { OrderStatus.Confirmed, OrderStatus.Cancelled },
            OrderStatus.Confirmed => new[] { OrderStatus.Shipping,  OrderStatus.Cancelled },
            OrderStatus.Shipping  => new[] { OrderStatus.Delivered, OrderStatus.Cancelled },
            _                     => Array.Empty<string>(),
        };

        if (!allowed.Contains(newStatus))
        {
            TempData["Error"] = $"Không thể chuyển từ '{order.Status}' sang '{newStatus}'.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        order.Status = newStatus;
        await _db.SaveChangesAsync();

        TempData["Success"] = $"✅ Đã cập nhật đơn #{id} → {newStatus}.";
        return RedirectToAction(nameof(Detail), new { id });
    }
}
