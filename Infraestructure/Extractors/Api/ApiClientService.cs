using System.Net.Http.Json;

namespace Infrastructure.Extractors.API
{
    public class ApiClientService
    {
        private readonly HttpClient _httpClient;

        public ApiClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<T>> GetAsync<T>(string url, CancellationToken cancellationToken = default)
        {
            var result = await _httpClient.GetFromJsonAsync<List<T>>(url, cancellationToken);
            return result ?? new List<T>();
        }
    }
}
