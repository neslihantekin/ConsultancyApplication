using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConsultancyApplication.Core.Interfaces.Services;

namespace ConsultancyApplication.Web.Controllers
{
    public class CurrentEndexesController : Controller
    {
        private readonly ICurrentEndexesService _currentEndexesService;

        public CurrentEndexesController(ICurrentEndexesService currentEndexesService)
        {
            _currentEndexesService = currentEndexesService;
        }

        public async Task<IActionResult> Index(long ownerSerno = 150071, int definitionType = 2, int endexDirection = 0)
        {
            // Tarihleri örnek veriyoruz (yyyyMMddHHmmss formatında)
            var startDate = "20230101000000";
            var endDate = "20230131235959";

            var currentEndexes = await _currentEndexesService.GetCurrentEndexesAsync(startDate, endDate, endexDirection);

            return View(currentEndexes);
        }
    }
}
