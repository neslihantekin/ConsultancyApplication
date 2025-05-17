using System.Net;

namespace ConsultancyApplication.Infrastructure.APIClients
{
    public class ApiClient
    {

        private readonly HttpClient _httpClient;

        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // Gerekirse _httpClient.BaseAddress = new Uri("http://IP/aril-portalserver/"); gibi
        }

        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content, Dictionary<string, string> headers = null)
        {                     
            HttpClientHandler handler = new HttpClientHandler
            {
                AllowAutoRedirect = false // Yönlendirmeleri otomatik yapma, manuel kontrol edelim.
            };
            using var client = new HttpClient(handler);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    if (!client.DefaultRequestHeaders.Contains(header.Key))
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
            }

            var request = new HttpRequestMessage(HttpMethod.Post, url);

            // POST isteği
            HttpResponseMessage response = await client.SendAsync(request);

            // Eğer yönlendirme varsa, aynı POST metodunu koruyarak tekrar isteği gönder
            if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
            {
                if (response.Headers.Location != null)
                {
                    string redirectUrl = response.Headers.Location.ToString();

                    // 307 ve 308 yönlendirmelerinde POST metodunu koruyarak tekrar istek yap
                    if (response.StatusCode == HttpStatusCode.TemporaryRedirect ||
                        response.StatusCode == HttpStatusCode.PermanentRedirect ||
                        response.StatusCode == HttpStatusCode.Found)
                    {
                        using var newRequest = new HttpRequestMessage(HttpMethod.Post, redirectUrl)
                        {
                            Content = content
                        };
                        response = await client.SendAsync(newRequest);
                    }
                }
            }
            // Gerekirse headerları temizle (reuse scenario)
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Remove(header.Key);
                }
            }

            return response;
        }
    }
}
