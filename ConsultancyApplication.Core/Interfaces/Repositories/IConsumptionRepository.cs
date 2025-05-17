using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultancyApplication.Core.Entities;

namespace ConsultancyApplication.Core.Interfaces.Repositories
{
    public interface IConsumptionRepository : IBaseRepository<Consumption>
    {
        Task<IEnumerable<Consumption>> GetAllByOwnerSernoAsync(long ownerSerno);
        Task SaveConsumptionsAsync(IEnumerable<Consumption> consumptions);
    }
}
