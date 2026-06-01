using Microsoft.AspNetCore.Mvc;

namespace HKTech.Controllers
{
    /// <summary>
    /// Controller cho trang danh sách linh kiện.
    /// Phase 2 sẽ inject ProductService và bind dữ liệu từ DB.
    /// </summary>
    public class ProductController : Controller
    {
        [HttpGet]
        public IActionResult Index(string? category, string? search, string? sort,
                                   decimal? priceMin, decimal? priceMax)
        {
            ViewBag.Category = category;
            ViewBag.Search   = search;
            // TODO Phase 2: truy vấn DB, trả về List<Product>
            return View();
        }

        [HttpGet]
        public IActionResult Detail(int id)
        {
            // TODO Phase 2: tìm Product theo id
            return View();
        }
    }
}
