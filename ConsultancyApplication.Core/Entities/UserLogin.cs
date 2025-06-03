using System;
using System.ComponentModel.DataAnnotations;

namespace ConsultancyApplication.Core.Entities
{
    public class UserLogin
    {
        public int Id { get; set; }  // Primary Key
        public string UserCode { get; set; }  // Kullanıcı adı
        public string Password { get; set; }  // Şifre         
        //public string AppPassword { get; set; }  // Şifre
        
        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
