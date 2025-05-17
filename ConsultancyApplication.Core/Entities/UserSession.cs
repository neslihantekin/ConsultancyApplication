using System;
namespace ConsultancyApplication.Core.Entities
{
    public class UserSession
    {
        public string Username { get; set; }   // Kullanıcı adı (UserCode)
        public string Password { get; set; }   // Kullanıcı şifresi
        public string AccessToken { get; set; } // Alınan token
        public DateTime TokenExpireTime { get; set; } // Token'ın süresi
        public long OwnerSerno { get; set; }           // 👈 Abonelikten gelen Serno
        public int DefinitionType { get; set; }        // 👈 Abonelikten gelen DefinitionType
        public string Title { get; set; }
        public decimal InstalledPower { get; set; }  //Kurulu Güç
    }
}