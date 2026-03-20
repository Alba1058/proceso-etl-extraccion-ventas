using Domain.Interfaces;
using Infrastructure.Extractors.API;

namespace Infrastructure.Extractors.API
{
    public class CustomerApiExtractor : IApiExtractor
    {
        private readonly ApiClientService _apiClient;

        public CustomerApiExtractor(ApiClientService apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<object>> ExtractAsync()
        {
            var data = await _apiClient.GetAsync<object>("https://api.example.com/customers");
            return data.ToList();
        }
    }
}