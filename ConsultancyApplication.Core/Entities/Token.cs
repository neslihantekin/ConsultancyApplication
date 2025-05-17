using System;

namespace ConsultancyApplication.Core.Entities
{
    public class Token
    {
        public int Id { get; set; } // EF Core için Primary Key
        // API'nin döndürdüğü JWT token bilgisini temsil eder.
        public string AccessToken { get; set; }

        // Token'ın geçerlilik süresini veya biteceği zamanı saklayabilirsiniz.
        // Doküman net bir Expiration dönmediği için opsiyoneldir, pratik kullanımda yararlı olur.
        public DateTime Expiration { get; set; }
    }
}
