using System.Linq;
using System.Threading.Tasks;
using ConsultancyApplication.Core.Entities;
using ConsultancyApplication.Core.Interfaces.Repositories;
using ConsultancyApplication.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConsultancyApplication.Infrastructure.Repositories
{
    public class TokenRepository : BaseRepository<Token>, ITokenRepository
    {
        public TokenRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task SaveTokenAsync(Token token)
        {
            // Örneğin sadece tek bir token kaydı tutmak istiyorsak
            // önce tüm kayıtları silebiliriz.
            var allTokens = _dbSet.ToList();
            _dbSet.RemoveRange(allTokens);

            await AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<Token> GetTokenAsync()
        {
            return await _dbSet.FirstOrDefaultAsync();
        }
    }
}
