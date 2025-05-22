using ConsultancyApplication.Core.Entities;
using ConsultancyApplication.Core.Interfaces.Repositories;
using ConsultancyApplication.Core.Interfaces.Services;
using ConsultancyApplication.Core.Utilities.Converters;
using ConsultancyApplication.Infrastructure.APIClients;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConsultancyApplication.Infrastructure.Services
{
    public class ConsumptionService : IConsumptionService
    {
        private readonly ApiClient _apiClient;
        private readonly IConsumptionRepository _consumptionRepository;
        private readonly ITokenService _tokenService;
        private readonly UserSession _userSession;

        public ConsumptionService(ApiClient apiClient, IConsumptionRepository consumptionRepository, ITokenService tokenService, UserSession userSession)
        {
            _apiClient = apiClient;
            _consumptionRepository = consumptionRepository;
            _tokenService = tokenService;
            _userSession = userSession;
        }

        public async Task<IEnumerable<Consumption>> GetOwnerConsumptionsAsync(string startDate, string endDate, int endexDirection = 0)
        {
            // Token kontrol
            var token = await _tokenService.GenerateTokenAsync();

            var url = "https://ososportal.bedas.com.tr/aril-portalserver/customer-rest-api/proxy-aril/GetOwnerConsumptions";
            
            // 👤 Kullanıcıya ait serno ve definition type oturumdan geliyor
            var ownerSerno = _userSession.OwnerSerno;
            var ownerType = _userSession.DefinitionType;
            
            var payloadObj = new
            {
                OwnerSerno = ownerSerno.ToString(),
                StartDate = startDate,    // yyyyMMddHHmmss formatında
                EndDate = endDate,
                OwnerType = ownerType,    // Abone: 2
                IncludeLoadProfiles = false,
                WithoutMultiplier = false,
                MergeResult = true
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
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = JsonSerializer.Deserialize<ConsumptionResponse>(responseBody, options);
                if (result != null)
                {
                    // Burada "MergedConsumptions", "InConsumption" vb. parse edilebilir.
                    // Biz basit bir örnek ile consumption listesi oluşturabiliriz.
                    var consumptions = result.ToConsumptionList(ownerSerno, ownerType);

                    // Veritabanına kaydet
                    //await _consumptionRepository.SaveConsumptionsAsync(consumptions);
                    return consumptions;
                }
                return new List<Consumption>();
            }
            else
            {
                throw new Exception($"Tüketim verileri alınamadı. StatusCode: {response.StatusCode}");
            }
        }

        // Yardımcı sınıflar
        private class ConsumptionResponse
        {
            public long OwnerSerno { get; set; }
            public int OwnerType { get; set; }
            public List<ConsumptionDetail> MergedConsumptions { get; set; }

            public List<Consumption> ToConsumptionList(long ownerSerno, int ownerType)
            {
                var list = new List<Consumption>();
                if (MergedConsumptions != null)
                {
                    foreach (var detail in MergedConsumptions)
                    {
                        list.Add(new Consumption
                        {
                            OwnerSerno = ownerSerno,
                            OwnerType = ownerType,
                            ProfileDate = detail.ProfileDate,
                            ActiveConsumption = detail.ActiveConsumption,
                            ActiveGeneration = detail.ActiveGeneration,
                            ReactiveInductive = detail.ReactiveInductive,
                            ReactiveCapacitive = detail.ReactiveCapacitive,
                            ReactiveInductiveOut = detail.ReactiveInductiveOut,
                            ReactiveCapacitiveOut = detail.ReactiveCapacitiveOut,
                            AdditionalConsumption = detail.AdditionalConsumption,
                            AdditionalGeneration = detail.AdditionalGeneration,
                            ReactiveInductiveRatio = detail.ReactiveInductiveRatio,
                            ReactiveCapacitiveRatio = detail.ReactiveCapacitiveRatio,
                            ReactiveInductiveOutRatio = detail.ReactiveInductiveOutRatio,
                            ReactiveCapacitiveOutRatio = detail.ReactiveCapacitiveOutRatio,
                            Multiplier = detail.Multiplier,
                            Status = detail.Status
                        });
                    }
                }
                return list;
            }
        }
        private class ConsumptionDetail
        {
            [JsonPropertyName("Pd")]
            [JsonConverter(typeof(ArilDateTimeConverter))]
            public DateTime? ProfileDate { get; set; }

            [JsonPropertyName("Cn")]
            public decimal? ActiveConsumption { get; set; }

            [JsonPropertyName("Gn")]
            public decimal? ActiveGeneration { get; set; }

            [JsonPropertyName("Ri")]
            public decimal? ReactiveInductive { get; set; }

            [JsonPropertyName("Rc")]
            public decimal? ReactiveCapacitive { get; set; }

            [JsonPropertyName("Rio")]
            public decimal? ReactiveInductiveOut { get; set; }

            [JsonPropertyName("Rco")]
            public decimal? ReactiveCapacitiveOut { get; set; }

            [JsonPropertyName("Addcn")]
            public decimal? AdditionalConsumption { get; set; }

            [JsonPropertyName("Addgn")]
            public decimal? AdditionalGeneration { get; set; }

            [JsonPropertyName("Rir")]
            public decimal? ReactiveInductiveRatio { get; set; }

            [JsonPropertyName("Rcr")]
            public decimal? ReactiveCapacitiveRatio { get; set; }

            [JsonPropertyName("Riro")]
            public decimal? ReactiveInductiveOutRatio { get; set; }

            [JsonPropertyName("Rcor")]
            public decimal? ReactiveCapacitiveOutRatio { get; set; }

            [JsonPropertyName("Ml")]
            public decimal? Multiplier { get; set; }

            [JsonPropertyName("St")]
            public int? Status { get; set; }
        }


    }
}
