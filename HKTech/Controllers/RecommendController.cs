using Microsoft.AspNetCore.Mvc;

namespace HKTech.Controllers
{
    /// <summary>
    /// Controller cho trang Gợi ý cấu hình PC.
    /// Phase 3 sẽ bổ sung thuật toán query combo linh kiện theo budget.
    /// </summary>
    public class RecommendController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string budget, string useCase, int ramPref, string? notes)
        {
            // TODO Phase 3: Query DB để tìm combo linh kiện phù hợp
            ViewBag.Budget  = budget;
            ViewBag.UseCase = useCase;
            ViewBag.RamPref = ramPref;
            return View();
        }
    }
}
