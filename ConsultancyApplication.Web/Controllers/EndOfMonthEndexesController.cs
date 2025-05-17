using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConsultancyApplication.Core.Interfaces.Services;

namespace ConsultancyApplication.Web.Controllers
{
    public class EndOfMonthEndexesController : Controller
    {
        private readonly IEndOfMonthEndexesService _endOfMonthEndexesService;

        public EndOfMonthEndexesController(IEndOfMonthEndexesService endOfMonthEndexesService)
        {
            _endOfMonthEndexesService = endOfMonthEndexesService;
        }

        public async Task<IActionResult> Index(long ownerSerno = 150071, int definitionType = 2, int endexDirection = 0)
        {
            // Tarihleri örnek veriyoruz (yyyyMMddHHmmss formatında)
            var startDate = "20230101000000";
            var endDate = "20230131235959";

            var eomEndexes = await _endOfMonthEndexesService.GetEndOfMonthEndexesAsync(startDate, endDate, endexDirection);

            return View(eomEndexes);
        }
    }
}
