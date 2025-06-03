using ConsultancyApplication.Core.Entities;
using ConsultancyApplication.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ConsultancyApplication.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ITokenService _tokenService;
        private readonly UserSession _userSession;
        private readonly ICustomerService _customerService;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(ITokenService tokenService, UserSession userSession, ICustomerService customerService, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _tokenService = tokenService;
            _userSession = userSession;
            _customerService = customerService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var model = new UserLogin();

            if (Request.Cookies["SavedUserCode"] != null)
            {
                model.UserCode = Request.Cookies["SavedUserCode"];
                model.Password = Request.Cookies["SavedPassword"];
                //model.AppPassword = Request.Cookies["SavedAppPassword"];
            }

            return View(model);
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
                // 🔐 1. Admin giriş kontrolü (ASP.NET Identity)
                if (userLogin.UserCode == "admin@demo.com")
                {
                    var result = await _signInManager.PasswordSignInAsync(userLogin.UserCode, userLogin.Password, false, false);
                    if (result.Succeeded)
                    {
                        var user = await _userManager.FindByEmailAsync(userLogin.UserCode);
                        var roles = await _userManager.GetRolesAsync(user);

                        if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("ClientCredentialManager", "Admin");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Bu kullanıcı admin rolüne sahip değil.");
                            return View(userLogin);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Admin girişi başarısız.");
                        return View(userLogin);
                    }
                }

                // 🔐 2. Diğer kullanıcılar için klasik token tabanlı oturum açma
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
                    //Response.Cookies.Append("SavedAppPassword", userLogin.AppPassword, cookieOptions);
                }
                else
                {
                    Response.Cookies.Delete("SavedUserCode");
                    Response.Cookies.Delete("SavedPassword");
                    Response.Cookies.Delete("SavedAppPassword");
                }

                // Token alma
                Token token = await _tokenService.GenerateTokenAsync();
                _userSession.AccessToken = token.AccessToken;
                _userSession.TokenExpireTime = token.Expiration;

                // Kullanıcı bilgileri
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

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _userSession.Reset(); // Kendi session'ını temizliyorsan burası yeterli
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return Content("Bu sayfaya erişim izniniz yok.");
        }
    }
}
