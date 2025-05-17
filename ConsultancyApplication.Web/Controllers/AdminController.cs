using Microsoft.AspNetCore.Mvc;
using ConsultancyApplication.Infrastructure.Data;
using System;
using System.Linq;
using ConsultancyApplication.Core.Entities;

namespace ConsultancyApplication.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
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
        public JsonResult DeleteClientCredential(int id)
        {
            var entity = _context.ClientCredentials.Find(id);
            if (entity == null)
                return Json(new { success = false });

            _context.ClientCredentials.Remove(entity);
            _context.SaveChanges();
            return Json(new { success = true });
        }
    }
}
