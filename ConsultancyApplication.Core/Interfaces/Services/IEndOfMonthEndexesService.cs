using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultancyApplication.Core.Entities;

namespace ConsultancyApplication.Core.Interfaces.Services
{
    public interface IEndOfMonthEndexesService
    {
        // Ay sonu endeks verilerinin alınması
        Task<IEnumerable<EndOfMonthEndexes>> GetEndOfMonthEndexesAsync(string startDate, string endDate, int endexDirection = 0);
    }
}
