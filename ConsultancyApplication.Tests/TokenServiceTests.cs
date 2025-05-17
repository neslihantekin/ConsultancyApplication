using System.Threading.Tasks;
using Xunit;
using Moq;
using ConsultancyApplication.Core.Interfaces.Repositories;
using ConsultancyApplication.Infrastructure.Services;
using ConsultancyApplication.Infrastructure.APIClients;

namespace ConsultancyApplication.Tests
{
    public class TokenServiceTests
    {
        [Fact]
        public async Task GenerateTokenAsync_ShouldReturnToken()
        {
            // Arrange
            var mockApiClient = new Mock<ApiClient>(new HttpClient());
            var mockTokenRepo = new Mock<ITokenRepository>();

            // mockApiClient Setup: Her çağrıda başarılı dönecek şekilde ayarlayabilirsiniz.

            var tokenService = new TokenService(mockApiClient.Object, mockTokenRepo.Object);

            // Act
            // "screet" usercode, "screet" password
            // Bu testte gerçek API'ye gitmiyoruz, bu nedenle mock'lamanız gerekir.
            // Sadece iskelet bir örnek.
            var token = await tokenService.GenerateTokenAsync("screet", "screet");

            // Assert
            Assert.NotNull(token);
            // Diğer assertler...
        }
    }
}
