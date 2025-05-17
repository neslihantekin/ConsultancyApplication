using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultancyApplication.Core.Entities;
using ConsultancyApplication.Core.Interfaces.Repositories;
using ConsultancyApplication.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConsultancyApplication.Infrastructure.Repositories
{
    public class ConsumptionRepository : BaseRepository<Consumption>, IConsumptionRepository
    {
        public ConsumptionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Consumption>> GetAllByOwnerSernoAsync(long ownerSerno)
        {
            return await _dbSet
                .Where(c => c.OwnerSerno == ownerSerno)
                .ToListAsync();
        }

        public async Task SaveConsumptionsAsync(IEnumerable<Consumption> consumptions)
        {
            await AddRangeAsync(consumptions);
            await _context.SaveChangesAsync();
        }
    }
}
