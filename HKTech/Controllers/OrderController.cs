// ================================================================
// HKTECH — OrderController.cs
// Phase 3: Checkout, Tạo Order, Xác nhận, Lịch sử đơn hàng
// ================================================================

using HKTech.Data;
using HKTech.Models;
using HKTech.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HKTech.Controllers
{
    [Authorize]   // Toàn bộ Order yêu cầu đăng nhập
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext    _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CartService             _cart;

        public OrderController(ApplicationDbContext db,
                               UserManager<ApplicationUser> userManager,
                               CartService cart)
        {
            _db          = db;
            _userManager = userManager;
            _cart        = cart;
        }

        // ================================================================
        // GET /Order/Checkout — Trang thanh toán
        // ================================================================
        public async Task<IActionResult> Checkout()
        {
            ViewData["Title"] = "Thanh toán";
            var items = _cart.GetCart();
            if (!items.Any())
                return RedirectToAction("Index", "Cart");

            var user = await _userManager.GetUserAsync(User);
            ViewBag.CartItems = items;
            ViewBag.Total     = _cart.GetTotal();
            ViewBag.UserName  = user?.FullName ?? user?.Email;
            return View();
        }

        // ================================================================
        // POST /Order/Checkout — Tạo Order + OrderDetail từ cart
        // ================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(string shippingAddress, string? note = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var cartItems = _cart.GetCart();
            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            // Lấy thông tin sản phẩm từ DB — chỉ lấy sản phẩm còn hoạt động
            var productIds = cartItems.Select(c => c.ProductId).ToList();
            var products   = await _db.Products
                .Where(p => productIds.Contains(p.Id) && p.IsActive)
                .ToDictionaryAsync(p => p.Id);

            // Lọc ra các item có sản phẩm đã bị xóa hoặc inactive
            var validItems = cartItems.Where(c => products.ContainsKey(c.ProductId)).ToList();
            if (!validItems.Any())
            {
                TempData["Error"] = "Tất cả sản phẩm trong giỏ hàng không còn được bán. Vui lòng kiểm tra lại giỏ hàng.";
                return RedirectToAction("Index", "Cart");
            }
            var removedCount = cartItems.Count - validItems.Count;

            // Tạo Order chỉ với sản phẩm hợp lệ
            var order = new Order
            {
                UserId          = user.Id,
                TotalPrice      = validItems.Sum(c => c.Quantity * products[c.ProductId].Price),
                Status          = OrderStatus.Pending,
                ShippingAddress = shippingAddress,
                CreatedAt       = DateTime.UtcNow,
                OrderDetails    = validItems.Select(c => new OrderDetail
                {
                    ProductId = c.ProductId,
                    Quantity  = c.Quantity,
                    UnitPrice = products[c.ProductId].Price,
                }).ToList()
            };
            if (removedCount > 0)
                TempData["Warn"] = $"⚠ {removedCount} sản phẩm đã ngừng bán đã được xóa khỏi đơn hàng.";

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // Xóa giỏ hàng sau khi đặt hàng thành công
            _cart.ClearCart();

            return RedirectToAction("Confirm", new { id = order.Id });
        }

        // ================================================================
        // GET /Order/Confirm/{id} — Trang xác nhận đơn hàng
        // ================================================================
        public async Task<IActionResult> Confirm(int id)
        {
            ViewData["Title"] = "Xác nhận đơn hàng";
            var user = await _userManager.GetUserAsync(User);

            var order = await _db.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user!.Id);

            if (order == null) return NotFound();
            return View(order);
        }

        // ================================================================
        // GET /Order/History — Lịch sử đơn hàng của user
        // ================================================================
        public async Task<IActionResult> History()
        {
            ViewData["Title"] = "Lịch sử đơn hàng";
            var user = await _userManager.GetUserAsync(User);

            var orders = await _db.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Where(o => o.UserId == user!.Id)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        // ================================================================
        // POST /Order/Cancel/{id} — Huỷ đơn hàng (chỉ khi Pending)
        // ================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var user  = await _userManager.GetUserAsync(User);
            var order = await _db.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user!.Id);

            if (order == null)
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });

            if (order.Status != OrderStatus.Pending)
                return Json(new { success = false, message = "Chỉ có thể huỷ đơn đang chờ xử lý." });

            order.Status = OrderStatus.Cancelled;
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "Đã huỷ đơn hàng." });
        }
    }
}
