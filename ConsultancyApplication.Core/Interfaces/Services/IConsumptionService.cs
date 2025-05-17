using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultancyApplication.Core.Entities;

namespace ConsultancyApplication.Core.Interfaces.Services
{
    public interface IConsumptionService
    {
        // Tekil tüketim verilerinin alınması
        Task<IEnumerable<Consumption>> GetOwnerConsumptionsAsync(string startDate, string endDate, int endexDirection = 0);
    }
}
