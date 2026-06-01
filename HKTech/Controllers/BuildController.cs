using Microsoft.AspNetCore.Mvc;

namespace HKTech.Controllers
{
    /// <summary>
    /// Controller cho trang Tự Build PC.
    /// Phase 3-4 sẽ bổ sung logic chọn linh kiện và tương thích.
    /// </summary>
    public class BuildController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Tự Build PC";
            return View();
        }
    }
}
