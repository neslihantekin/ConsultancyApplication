using ConsultancyApplication.Core.Entities;
using ConsultancyApplication.Core.Interfaces.Repositories;
using ConsultancyApplication.Core.Interfaces.Services;
using ConsultancyApplication.Infrastructure.APIClients;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ConsultancyApplication.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly ApiClient _apiClient;
        private readonly ITokenRepository _tokenRepository;
        Token token = new Token();
        private readonly UserSession _userSession;

        public TokenService(ApiClient apiClient, ITokenRepository tokenRepository, UserSession userSession)
        {
            _apiClient = apiClient;
            _tokenRepository = tokenRepository;
            _userSession = userSession;
        }

        public async Task<Token> GenerateTokenAsync()
        {
            if (!string.IsNullOrEmpty(_userSession.AccessToken) && _userSession.TokenExpireTime > DateTime.Now)
            {
                Token newToken = new Token();
                newToken.AccessToken = _userSession.AccessToken;
                newToken.Expiration = _userSession.TokenExpireTime;
                return newToken;
            }
            // Dokümanda: http://IP/aril-portalserver/customer-rest-api/generate-token
            var url = "https://ososportal.bedas.com.tr/aril-portalserver/customer-rest-api/generate-token";

            // Gönderilecek JSON gövdesi
            var payloadObj = new
            {
                UserCode = _userSession.Username,
                Password = _userSession.Password
            };
            var jsonPayload = JsonSerializer.Serialize(payloadObj);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            try
            {
                // Post isteği              
                var response = await _apiClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();

                    var tokenString = responseBody.Trim('"'); // Eğer çift tırnaklar varsa temizle
                    token.AccessToken = tokenString;

                    // Opsiyonel: Token geçerlilik süresi 20dk
                    token.Expiration = DateTime.Now.AddMinutes(20);

                    // Token veritabanına kaydedilebilir.
                    //await _tokenRepository.SaveTokenAsync(token);
                    return token;
                }
                else
                {
                    // Hata yönetimi
                    string errorContent = await response.Content.ReadAsStringAsync();
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            throw new Exception($"Geçersiz istek. Error: {errorContent}");
                        case HttpStatusCode.Unauthorized:
                            throw new Exception("Yetkilendirme hatası! Kullanıcı kodu veya şifre yanlış.");
                        case HttpStatusCode.Forbidden:
                            throw new Exception("Erişim engellendi! Yetkiniz yok.");
                        case HttpStatusCode.InternalServerError:
                            throw new Exception("Sunucu hatası! Daha sonra tekrar deneyiniz.");
                        default:
                            throw new Exception($"Beklenmeyen hata: {response.StatusCode}, Response: {errorContent}");
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception("API isteği sırasında bir hata oluştu: " + httpEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Token oluşturulurken bir hata meydana geldi: " + ex.Message);
            }
        }
    }
}
