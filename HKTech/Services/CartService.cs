// ================================================================
// HKTECH — CartService.cs
// Session-based cart service, inject vào Controller qua DI
// ================================================================

using HKTech.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HKTech.Services
{
    // ── Cart Item ViewModel ───────────────────────────────────────
    public class CartItem
    {
        public int     ProductId { get; set; }
        public string  Name      { get; set; } = string.Empty;
        public decimal Price     { get; set; }
        public int     Quantity  { get; set; } = 1;
        public string  ImageUrl  { get; set; } = string.Empty;
        public string  Slug      { get; set; } = string.Empty; // category slug

        public decimal Subtotal => Price * Quantity;
    }

    // ── Cart Service ──────────────────────────────────────────────
    public class CartService
    {
        private const string CART_KEY = "HKTechCart";
        private readonly IHttpContextAccessor _httpContext;
        private readonly ApplicationDbContext _db;

        public CartService(IHttpContextAccessor httpContext, ApplicationDbContext db)
        {
            _httpContext = httpContext;
            _db          = db;
        }

        // ── Đọc cart từ Session ──────────────────────────────────
        public List<CartItem> GetCart()
        {
            var session = _httpContext.HttpContext?.Session;
            if (session == null) return new();
            var json = session.GetString(CART_KEY);
            if (string.IsNullOrEmpty(json)) return new();
            return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new();
        }

        // ── Lưu cart vào Session ─────────────────────────────────
        private void SaveCart(List<CartItem> cart)
        {
            _httpContext.HttpContext?.Session.SetString(
                CART_KEY, JsonSerializer.Serialize(cart));
        }

        // ── Thêm sản phẩm ────────────────────────────────────────
        public async Task<bool> AddItemAsync(int productId, int quantity = 1)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);

            if (product == null) return false;

            var cart = GetCart();
            var existing = cart.FirstOrDefault(c => c.ProductId == productId);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    Name      = product.Name,
                    Price     = product.Price,
                    Quantity  = quantity,
                    ImageUrl  = product.PrimaryImageUrl ?? "/images/placeholder.png",
                    Slug      = product.Category.Slug,
                });
            }

            SaveCart(cart);
            return true;
        }

        // ── Thêm toàn bộ build vào cart ──────────────────────────
        public async Task<int> AddBuildToCartAsync(List<int> productIds)
        {
            int added = 0;
            foreach (var id in productIds)
            {
                if (await AddItemAsync(id)) added++;
            }
            return added;
        }

        // ── Cập nhật số lượng ────────────────────────────────────
        public bool UpdateQuantity(int productId, int quantity)
        {
            if (quantity <= 0) return RemoveItem(productId);
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item == null) return false;
            item.Quantity = quantity;
            SaveCart(cart);
            return true;
        }

        // ── Xóa 1 sản phẩm ───────────────────────────────────────
        public bool RemoveItem(int productId)
        {
            var cart = GetCart();
            var removed = cart.RemoveAll(c => c.ProductId == productId) > 0;
            SaveCart(cart);
            return removed;
        }

        // ── Xóa toàn bộ cart ─────────────────────────────────────
        public void ClearCart()
        {
            _httpContext.HttpContext?.Session.Remove(CART_KEY);
        }

        // ── Tổng số lượng items ───────────────────────────────────
        public int GetItemCount() => GetCart().Sum(c => c.Quantity);

        // ── Tổng tiền ─────────────────────────────────────────────
        public decimal GetTotal() => GetCart().Sum(c => c.Subtotal);
    }
}
