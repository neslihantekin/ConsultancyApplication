using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace ConsultancyApplication.Infrastructure.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Mevcut dizini al
            var basePath = Directory.GetCurrentDirectory();

            // Web projesindeki appsettings.json'a ulaş
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile(Path.Combine("..", "ConsultancyApplication.Web", "appsettings.json"), optional: true)
                .Build();

            // Bağlantı cümlesini al
            var connectionString = config.GetConnectionString("DefaultConnection");

            // DbContextOptions yapılandır
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
