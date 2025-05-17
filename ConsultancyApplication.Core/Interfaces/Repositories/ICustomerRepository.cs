using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultancyApplication.Core.Entities;

namespace ConsultancyApplication.Core.Interfaces.Repositories
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        Task SaveSubscriptionsAsync(IEnumerable<Customer> customers);
        // İsteğe bağlı: Veritabanından okuyacak metotlar
        Task<IEnumerable<Customer>> GetAllAsync();
    }
}                                    
