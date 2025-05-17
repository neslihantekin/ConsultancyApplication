using System.Threading.Tasks;
using ConsultancyApplication.Core.Entities;

namespace ConsultancyApplication.Core.Interfaces.Repositories
{
    public interface ITokenRepository : IBaseRepository<Token>
    {
        Task<Token> GetTokenAsync();
        Task SaveTokenAsync(Token token);
    }
}
