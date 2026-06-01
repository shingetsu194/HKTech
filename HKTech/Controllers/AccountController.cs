using Microsoft.AspNetCore.Mvc;

namespace HKTech.Controllers
{
    /// <summary>
    /// Controller xử lý Đăng nhập / Đăng ký.
    /// Phase 2 sẽ tích hợp ASP.NET Core Identity.
    /// </summary>
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Profile()
        {
            // TODO Phase 2: Yêu cầu đăng nhập [Authorize]
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // TODO Phase 2: SignOut Identity
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult OrderHistory()
        {
            // TODO Phase 3: Lấy lịch sử đơn hàng
            return View();
        }
    }
}
