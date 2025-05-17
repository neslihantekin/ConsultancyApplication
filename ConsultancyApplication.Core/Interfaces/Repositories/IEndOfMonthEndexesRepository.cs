using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultancyApplication.Core.Entities;

namespace ConsultancyApplication.Core.Interfaces.Repositories
{
    public interface IEndOfMonthEndexesRepository : IBaseRepository<EndOfMonthEndexes>
    {
        // Belirli bir OwnerSerno için ay sonu endeks verilerini getirir.
        Task<IEnumerable<EndOfMonthEndexes>> GetAllByOwnerSernoAsync(long ownerSerno);

        // Ay sonu endeks verilerini veritabanına kaydeder.
        Task SaveEndOfMonthEndexesAsync(IEnumerable<EndOfMonthEndexes> endexes);
    }
}
