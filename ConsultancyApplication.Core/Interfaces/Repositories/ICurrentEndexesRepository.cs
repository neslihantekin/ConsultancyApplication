using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultancyApplication.Core.Entities;

namespace ConsultancyApplication.Core.Interfaces.Repositories
{
    public interface ICurrentEndexesRepository : IBaseRepository<CurrentEndexes>
    {        
        // Belirli bir OwnerSerno için endeks verilerini getirir.
        Task<IEnumerable<CurrentEndexes>> GetAllByOwnerSernoAsync(long ownerSerno);

        // Gelen endeks listesini veritabanına kaydeder.
        Task SaveCurrentEndexesAsync(IEnumerable<CurrentEndexes> endexes);
    }
}
