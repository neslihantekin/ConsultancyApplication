using System.Net;

namespace ConsultancyApplication.Infrastructure.APIClients
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content, Dictionary<string, string> headers = null)
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                AllowAutoRedirect = false // Yönlendirmeleri otomatik yapma, manuel kontrol edelim
            };
            using var client = new HttpClient(handler);

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            // ✅ Header'ları düzgün şekilde ekle
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    if (!client.DefaultRequestHeaders.Contains(header.Key))
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                    // aril-service-token gibi özel başlıklar için
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            HttpResponseMessage response;

            try
            {
                response = await client.SendAsync(request);

                // Eğer yönlendirme varsa, aynı POST metodunu koruyarak tekrar isteği gönder
                if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400 && response.Headers.Location != null)
                {
                    var redirectUrl = response.Headers.Location.ToString();

                    if (response.StatusCode == HttpStatusCode.TemporaryRedirect ||
                        response.StatusCode == HttpStatusCode.PermanentRedirect ||
                        response.StatusCode == HttpStatusCode.Found)
                    {
                        var newRequest = new HttpRequestMessage(HttpMethod.Post, redirectUrl)
                        {
                            Content = content
                        };

                        // 🔁 Header'ları yeniden ekle
                        if (headers != null)
                        {
                            foreach (var header in headers)
                            {
                                newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                            }
                        }

                        response = await client.SendAsync(newRequest);
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                // İsteğe bağlı loglama vs.
                throw new Exception($"HTTP POST hatası: {ex.Message}", ex);
            }
        }
    }
}
