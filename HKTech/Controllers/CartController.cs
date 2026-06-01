using Microsoft.AspNetCore.Mvc;

namespace HKTech.Controllers
{
    /// <summary>
    /// Controller cho Giỏ hàng.
    /// Phase 3 sẽ triển khai CartService (Session-based).
    /// </summary>
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            // TODO Phase 3: đọc cart từ Session
            return View();
        }
    }
}
