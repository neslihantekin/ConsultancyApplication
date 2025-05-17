using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConsultancyApplication.Core.Interfaces.Services;

namespace ConsultancyApplication.Web.Controllers
{
    public class ConsumptionController : Controller
    {
        private readonly IConsumptionService _consumptionService;

        public ConsumptionController(IConsumptionService consumptionService)
        {
            _consumptionService = consumptionService;
        }

        public async Task<IActionResult> Index(long ownerSerno = 150071, int ownerType = 2)
        {
            // Tarihleri örnek veriyoruz (yyyyMMddHHmmss formatında)
            var startDate = "20230101000000";
            var endDate = "20230131235959";

            var consumptions = await _consumptionService.GetOwnerConsumptionsAsync(startDate, endDate, 0);

            return View(consumptions);
        }
    }
}
