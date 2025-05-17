using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultancyApplication.Core.Entities;
using ConsultancyApplication.Core.Interfaces.Repositories;
using ConsultancyApplication.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConsultancyApplication.Infrastructure.Repositories
{
    public class CurrentEndexesRepository : BaseRepository<CurrentEndexes>, ICurrentEndexesRepository
    {
        public CurrentEndexesRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CurrentEndexes>> GetAllByOwnerSernoAsync(long ownerSerno)
        {
            return await _dbSet
                .Where(e => e.OwnerSerno == ownerSerno)
                .ToListAsync();
        }

        public async Task SaveCurrentEndexesAsync(IEnumerable<CurrentEndexes> endexes)
        {
            await AddRangeAsync(endexes);
            await _context.SaveChangesAsync();
        }
    }
}
