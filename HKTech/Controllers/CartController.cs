// ================================================================
// HKTECH — CartController.cs
// Phase 3: Xem, Thêm, Xóa, Cập nhật giỏ hàng (Session-based)
// ================================================================

using HKTech.Services;
using Microsoft.AspNetCore.Mvc;

namespace HKTech.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cart;

        public CartController(CartService cart)
        {
            _cart = cart;
        }

        // ================================================================
        // GET /Cart — Trang giỏ hàng
        // ================================================================
        public IActionResult Index()
        {
            ViewData["Title"] = "Giỏ hàng";
            var items = _cart.GetCart();
            ViewBag.Total     = _cart.GetTotal();
            ViewBag.ItemCount = _cart.GetItemCount();
            return View(items);
        }

        // ================================================================
        // POST /Cart/Add — Thêm sản phẩm vào giỏ
        // ================================================================
        [HttpPost]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            var ok = await _cart.AddItemAsync(productId, quantity);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success   = ok,
                    message   = ok ? "Đã thêm vào giỏ hàng!" : "Sản phẩm không tồn tại.",
                    cartCount = _cart.GetItemCount(),
                    cartTotal = _cart.GetTotal().ToString("N0") + " ₫",
                });
            }
            TempData["CartMsg"] = ok ? "Đã thêm vào giỏ hàng!" : "Lỗi khi thêm sản phẩm.";
            return RedirectToAction("Index");
        }

        // ================================================================
        // POST /Cart/AddBuild — Thêm toàn bộ build vào giỏ
        // ================================================================
        [HttpPost]
        public async Task<IActionResult> AddBuild([FromBody] List<int> productIds)
        {
            if (productIds == null || !productIds.Any())
                return Json(new { success = false, message = "Build đang trống." });

            var added = await _cart.AddBuildToCartAsync(productIds);
            return Json(new
            {
                success   = true,
                message   = $"Đã thêm {added} linh kiện vào giỏ hàng!",
                cartCount = _cart.GetItemCount(),
                cartTotal = _cart.GetTotal().ToString("N0") + " ₫",
            });
        }

        // ================================================================
        // POST /Cart/Update — Cập nhật số lượng
        // ================================================================
        [HttpPost]
        public IActionResult Update(int productId, int quantity)
        {
            _cart.UpdateQuantity(productId, quantity);
            return Json(new
            {
                success   = true,
                cartCount = _cart.GetItemCount(),
                cartTotal = _cart.GetTotal().ToString("N0") + " ₫",
                lineTotal = (_cart.GetCart()
                    .FirstOrDefault(c => c.ProductId == productId)?.Subtotal ?? 0)
                    .ToString("N0") + " ₫",
            });
        }

        // ================================================================
        // POST /Cart/Remove — Xóa 1 sản phẩm
        // ================================================================
        [HttpPost]
        public IActionResult Remove(int productId)
        {
            _cart.RemoveItem(productId);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success   = true,
                    cartCount = _cart.GetItemCount(),
                    cartTotal = _cart.GetTotal().ToString("N0") + " ₫",
                });
            }
            return RedirectToAction("Index");
        }

        // ================================================================
        // POST /Cart/Clear — Xóa toàn bộ giỏ hàng
        // ================================================================
        [HttpPost]
        public IActionResult Clear()
        {
            _cart.ClearCart();
            return RedirectToAction("Index");
        }

        // ================================================================
        // GET /Cart/Count — API lấy số lượng item (cho navbar badge)
        // ================================================================
        [HttpGet]
        public IActionResult Count()
        {
            return Json(new { count = _cart.GetItemCount() });
        }
    }
}
