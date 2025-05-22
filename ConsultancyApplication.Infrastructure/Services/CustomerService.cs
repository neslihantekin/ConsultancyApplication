using ConsultancyApplication.Core.Entities;
using ConsultancyApplication.Core.Interfaces.Repositories;
using ConsultancyApplication.Core.Interfaces.Services;
using ConsultancyApplication.Infrastructure.APIClients;
using System.Text;
using System.Text.Json;

namespace ConsultancyApplication.Infrastructure.Services
{
    // GetCustomerPortalSubscriptions servisini kullanır.
    public class CustomerService : ICustomerService
    {
        private readonly ApiClient _apiClient;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITokenService _tokenService;
        private readonly UserSession _userSession;

        public CustomerService(ApiClient apiClient, ICustomerRepository customerRepository, ITokenService tokenService, UserSession userSession)
        {
            _apiClient = apiClient;
            _customerRepository = customerRepository;
            _tokenService = tokenService;
            _userSession = userSession;
        }

        public async Task<IEnumerable<Customer>> GetSubscriptionsAsync(int pageNumber, int pageSize)
        {
            try
            {
                var token = await _tokenService.GenerateTokenAsync();
                if (token == null || token.Expiration < DateTime.Now)
                {
                    token = await _tokenService.GenerateTokenAsync();
                }

                var url = "https://ososportal.bedas.com.tr/aril-portalserver/customer-rest-api/proxy-aril/GetCustomerPortalSubscriptions";

                var payloadObj = new { PageNumber = pageNumber, PageSize = pageSize };
                var jsonPayload = JsonSerializer.Serialize(payloadObj);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var headers = new Dictionary<string, string>                 
                {
                    { "aril-service-token", token.AccessToken }
                };
                var response = await _apiClient.PostAsync(url, content, headers);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    var result = JsonSerializer.Deserialize<SubscriptionsResponse>(responseBody, options);

                    if (result?.ResultList != null && result.ResultList.Any())
                    {
                        var first = result.ResultList.First();

                        // 🔐 UserSession'a Serno ve DefinitionType kaydet
                        _userSession.OwnerSerno = first.SubscriptionSerno;
                        _userSession.DefinitionType = first.DefinitionType;

                        // Veritabanına kaydet
                        //await _customerRepository.SaveSubscriptionsAsync(result.ResultList);
                        return result.ResultList;
                    }
                    return new List<Customer>();
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API hatası: {response.StatusCode} - {errorBody}");
                }
            }
            catch (JsonException jsonEx)
            {
                throw new Exception("JSON dönüşüm hatası: " + jsonEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Abonelik bilgileri alınırken bir hata oluştu: " + ex.Message);
            }
        }

        // Yardımcı sınıf
        private class SubscriptionsResponse
        {
            public List<Customer> ResultList { get; set; }
        }
    }
}
