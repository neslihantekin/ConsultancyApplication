using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultancyApplication.Core.Entities;

namespace ConsultancyApplication.Core.Interfaces.Services
{
    public interface ICustomerService
    {
        // Tüketim noktalarını (abonelikleri) API'den çekip döner
        Task<IEnumerable<Customer>> GetSubscriptionsAsync(int pageNumber, int pageSize);
    }
}
