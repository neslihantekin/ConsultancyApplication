using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultancyApplication.Core.Entities;

namespace ConsultancyApplication.Core.Interfaces.Services
{
    public interface ICurrentEndexesService
    {
        // Mevcut endeks verilerinin alınması
        Task<IEnumerable<CurrentEndexes>> GetCurrentEndexesAsync(string startDate, string endDate, int endexDirection = 0);
    }
}
