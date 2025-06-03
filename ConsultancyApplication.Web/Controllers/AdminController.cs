using ConsultancyApplication.Core.Entities;
using ConsultancyApplication.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ConsultancyApplication.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult TabulatorTest()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View(); // View sadece grid içerecek
        }
        [HttpGet]
        public IActionResult ClientCredentialManager()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetClientCredentials()
        {
            var list = _context.ClientCredentials.OrderByDescending(x => x.CreatedAt).ToList();
            return Json(list);
        }

        [HttpPost]
        public JsonResult AddClientCredential([FromBody] ClientCredential model)
        {
            model.CreatedAt = DateTime.Now;
            _context.ClientCredentials.Add(model);
            _context.SaveChanges();
            return Json(new { success = true, data = model });
        }

        [HttpPost]
        public JsonResult UpdateClientCredential([FromBody] ClientCredential model)
        {
            var entity = _context.ClientCredentials.Find(model.Id);
            if (entity == null)
                return Json(new { success = false });

            entity.CompanyName = model.CompanyName;
            entity.ContactPerson = model.ContactPerson;
            entity.PhoneNumber = model.PhoneNumber;
            entity.Email = model.Email;
            entity.PortalUsername = model.PortalUsername;
            entity.PortalPassword = model.PortalPassword;
            entity.AppPassword = model.AppPassword;

            _context.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult DeleteClientCredential([FromBody] JsonElement data)
        {
            if (!data.TryGetProperty("id", out var idElement) || !idElement.TryGetInt32(out int id))
            {
                return Json(new { success = false, message = "ID alınamadı." });
            }

            var entity = _context.ClientCredentials.Find(id);
            if (entity == null)
                return Json(new { success = false });

            _context.ClientCredentials.Remove(entity);
            _context.SaveChanges();
            return Json(new { success = true });
        }
    }
}
