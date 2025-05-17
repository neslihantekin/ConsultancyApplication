using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConsultancyApplication.Core.Interfaces.Services;

namespace ConsultancyApplication.Web.Controllers
{
    public class TokenController : Controller
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        // Örnek: Token oluşturmayı tetikleyen bir action
        public async Task<IActionResult> Index()
        {
            var token = await _tokenService.GenerateTokenAsync();
            ViewBag.Token = token.AccessToken;
            return View();
        }
    }
}
