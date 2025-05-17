using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultancyApplication.Core.Entities;
using ConsultancyApplication.Core.Interfaces.Repositories;
using ConsultancyApplication.Infrastructure.Data;

namespace ConsultancyApplication.Infrastructure.Repositories
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task SaveSubscriptionsAsync(IEnumerable<Customer> customers)
        {
            // BaseRepository'den gelen AddRangeAsync metodu
            await AddRangeAsync(customers);

            // BaseRepository, SaveChangesAsync içermiyor varsayımıyla 
            // context üzerinde SaveChangesAsync çağırabilirsiniz.
            await _context.SaveChangesAsync();
        }

        // Örneğin ICustomerRepository içindeki 
        // Task<IEnumerable<Customer>> GetAllAsync(); 
        // zaten BaseRepository'deki GetAllAsync() metodu ile karşılanır.
        // Gerekirse override da yapabilirsiniz.
    }
}
