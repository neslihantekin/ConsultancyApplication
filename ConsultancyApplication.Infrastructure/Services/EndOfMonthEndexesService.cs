using ConsultancyApplication.Core.Entities;
using ConsultancyApplication.Core.Interfaces.Repositories;
using ConsultancyApplication.Core.Interfaces.Services;
using ConsultancyApplication.Infrastructure.APIClients;
using System.Text;
using System.Text.Json;

namespace ConsultancyApplication.Infrastructure.Services
{
    public class EndOfMonthEndexesService : IEndOfMonthEndexesService
    {
        private readonly ApiClient _apiClient;
        private readonly IEndOfMonthEndexesRepository _endOfMonthEndexesRepository;
        private readonly ITokenService _tokenService;
        private readonly UserSession _userSession;

        public EndOfMonthEndexesService(ApiClient apiClient, IEndOfMonthEndexesRepository endOfMonthEndexesRepository, ITokenService tokenService, UserSession userSession)
        {
            _apiClient = apiClient;
            _endOfMonthEndexesRepository = endOfMonthEndexesRepository;
            _tokenService = tokenService;
            _userSession = userSession;
        }

        public async Task<IEnumerable<EndOfMonthEndexes>> GetEndOfMonthEndexesAsync(string startDate, string endDate, int endexDirection = 0)
        {
            var token = await _tokenService.GenerateTokenAsync();

            var url = "https://ososportal.bedas.com.tr/aril-portalserver/customer-rest-api/proxy-aril/GetEndOfMonthEndexes";

            // 👤 Kullanıcıya ait serno ve definition type oturumdan geliyor
            var ownerSerno = _userSession.OwnerSerno;
            var definitionType = _userSession.DefinitionType;

            var payloadObj = new
            {
                OwnerSerno = ownerSerno.ToString(),
                DefinitionType = definitionType,
                EndexDirection = endexDirection,
                StartDate = startDate,
                EndDate = endDate
            };
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
                var result = JsonSerializer.Deserialize<EndOfMonthEndexesResponse>(responseBody, options);

                if (result?.ResultList != null)
                {
                    foreach (var item in result.ResultList)
                    {
                        item.OwnerSerno = ownerSerno; // Ek
                    }
                    //await _endOfMonthEndexesRepository.SaveEndOfMonthEndexesAsync(result.ResultList);
                }
                return result.ResultList;
            }
            else
            {
                throw new Exception($"Ay sonu endeks verileri alınamadı. StatusCode: {response.StatusCode}");
            }
        }

        private class EndOfMonthEndexesResponse
        {
            public List<EndOfMonthEndexes> ResultList { get; set; }
        }
    }
}
