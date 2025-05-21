using ConsultancyApplication.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConsultancyApplication.Infrastructure.Data
{
    // Identity işlemlerini de içeriyor, bu sayede hem ASP.NET Identity hem de domain entity'ler aynı DbContext üzerinden yönetiliyor.
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Domain entity'leriniz:
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Consumption> Consumptions { get; set; }
        public DbSet<CurrentEndexes> CurrentEndexes { get; set; }
        public DbSet<EndOfMonthEndexes> EndOfMonthEndexes { get; set; }
        public DbSet<ClientCredential> ClientCredentials { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Gerekli konfigürasyonları burada yapabilirsiniz.
        }
    }
}
