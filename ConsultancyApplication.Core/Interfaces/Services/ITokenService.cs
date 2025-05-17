using System.Threading.Tasks;
using ConsultancyApplication.Core.Entities;

namespace ConsultancyApplication.Core.Interfaces.Services
{
    public interface ITokenService
    {
        Task<Token> GenerateTokenAsync();
    }
}
