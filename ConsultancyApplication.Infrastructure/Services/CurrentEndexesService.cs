using ConsultancyApplication.Core.Entities;
using ConsultancyApplication.Core.Interfaces.Repositories;
using ConsultancyApplication.Core.Interfaces.Services;
using ConsultancyApplication.Infrastructure.APIClients;
using System.Text;
using System.Text.Json;

namespace ConsultancyApplication.Infrastructure.Services
{
    public class CurrentEndexesService : ICurrentEndexesService
    {
        private readonly ApiClient _apiClient;
        private readonly ICurrentEndexesRepository _currentEndexesRepository;
        private readonly ITokenService _tokenService;
        private readonly UserSession _userSession;

        public CurrentEndexesService(ApiClient apiClient, ICurrentEndexesRepository currentEndexesRepository, ITokenService tokenService, UserSession userSession)
        {
            _apiClient = apiClient;
            _currentEndexesRepository = currentEndexesRepository;
            _tokenService = tokenService;
            _userSession = userSession;
        }
        public async Task<IEnumerable<CurrentEndexes>> GetCurrentEndexesAsync(string startDate, string endDate,int endexDirection = 0) //
        {
            //EndexDirection (Endeks Yönü) : Çekiş(Tüketim): 0, Veriş(Üretim): 1

            try
            {
                var token = await _tokenService.GenerateTokenAsync();
                var url = "http://ososportal.bedas.com.tr/aril-portalserver/customer-rest-api/proxy-aril/GetCurrentEndexes";

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

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API hatası: StatusCode = {response.StatusCode}, Mesaj = {error}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var result = JsonSerializer.Deserialize<CurrentEndexesResponse>(responseBody, options);

                if (result?.ResultList == null || !result.ResultList.Any())
                    return new List<CurrentEndexes>(); // Boş liste döneriz

                // verileri işlemeye devam
                foreach (var item in result.ResultList)
                {
                    item.OwnerSerno = ownerSerno;
                }

                // DB'ye kaydet
                //await _currentEndexesRepository.SaveCurrentEndexesAsync(result.ResultList);
                return result.ResultList;
            }
            catch (JsonException jsonEx)
            {
                throw new Exception("JSON çözümleme hatası: API'den beklenmeyen formatta veri geldi. Detay: " + jsonEx.Message);
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception("HTTP isteği sırasında bağlantı hatası oluştu: " + httpEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Mevcut endeksler alınırken beklenmeyen bir hata oluştu: " + ex.Message);
            }
        }

        private class CurrentEndexesResponse
        {
            public List<CurrentEndexes> ResultList { get; set; }
        }
    }
}
