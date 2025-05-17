using ConsultancyApplication.Core.Entities;
using ConsultancyApplication.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConsultancyApplication.Web.Controllers
{
    public class AccountController : Controller
    {                               
        private readonly ITokenService _tokenService; 
        private readonly UserSession _userSession;
        private readonly ICustomerService _customerService;
        public AccountController(ITokenService tokenService, UserSession userSession, ICustomerService customerService)
        {
            _tokenService = tokenService;
            _userSession = userSession;
            _customerService = customerService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var model = new UserLogin();

            if (Request.Cookies["SavedUserCode"] != null)
            {
                model.UserCode = Request.Cookies["SavedUserCode"];
                model.Password = Request.Cookies["SavedPassword"];
                model.AppPassword = Request.Cookies["SavedAppPassword"];
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            if (!ModelState.IsValid)
            {
                // Model valid değilse, tekrar view'e dönüyoruz.
                return View(userLogin);
            }
            if (string.IsNullOrWhiteSpace(userLogin.UserCode) || string.IsNullOrWhiteSpace(userLogin.Password))
            {
                ModelState.AddModelError("", "Lütfen kullanıcı adı ve şifre giriniz.");
                return View();
            }
            try
            {                    
                // 🔐 Session'a bilgileri yaz
                _userSession.Username = userLogin.UserCode;
                _userSession.Password = userLogin.Password;

                // Cookie ekleme kısmı 👇
                if (userLogin.RememberMe)
                {
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(7),
                        HttpOnly = true,
                        Secure = true, // HTTPS'de çalışıyorsa önerilir
                        SameSite = SameSiteMode.Strict
                    };

                    Response.Cookies.Append("SavedUserCode", userLogin.UserCode, cookieOptions);
                    Response.Cookies.Append("SavedPassword", userLogin.Password, cookieOptions);
                    Response.Cookies.Append("SavedAppPassword", userLogin.AppPassword, cookieOptions);
                }
                else
                {
                    // Cookie varsa sil
                    if (Request.Cookies["SavedUserCode"] != null)
                    {
                        Response.Cookies.Delete("SavedUserCode");
                    }
                }

                // Token üretimi: API dokümanına göre token alınır.
                Token token = await _tokenService.GenerateTokenAsync();
                                                       
                _userSession.AccessToken = token.AccessToken;
                _userSession.TokenExpireTime = token.Expiration;

                // 🔁 Abonelikleri çağır ve ilk veriyi UserSession’a yazdır
                var subscriptions = await _customerService.GetSubscriptionsAsync(1, 1000);
                
                var title = subscriptions?.FirstOrDefault()?.Title ?? "Kullanıcı";
                var installedPower = subscriptions?.FirstOrDefault()?.InstalledPower ?? 0;
                _userSession.Title = title;
                _userSession.InstalledPower = installedPower;

                // Başarılıysa dashboard'a yönlendirme
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Giriş işlemi başarısız: " + ex.Message);
                return View(userLogin);
            }
        }
    }
}
