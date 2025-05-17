using Microsoft.AspNetCore.Mvc;

namespace ConsultancyApplication.Web.Controllers
{
    public class DashboardController : Controller
    {
        // GET: /Dashboard/Index
        public IActionResult Index()
        {
            // Örneğin, TempData ile login sırasında gönderilen token bilgisi varsa alabiliriz.
            ViewBag.AccessToken = TempData["AccessToken"] as string;
            return View();
        }
    }
}
