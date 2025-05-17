using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConsultancyApplication.Core.Interfaces.Services;

namespace ConsultancyApplication.Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<IActionResult> Index()
        {
            // Sayfa numarası ve boyutunu örnek veriyoruz.
            var customers = await _customerService.GetSubscriptionsAsync(1, 1000);
            return View(customers);
        }
    }
}
