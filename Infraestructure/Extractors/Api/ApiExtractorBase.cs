using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extractors.API
{
    public abstract class ApiExtractorBase<T> : IApiExtractor where T : class
    {
        private readonly ApiClientService _apiClientService;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        protected ApiExtractorBase(ApiClientService apiClientService, IConfiguration configuration, ILogger logger)
        {
            _apiClientService = apiClientService;
            _configuration = configuration;
            _logger = logger;
        }

        public abstract string SourceName { get; }
        public string SourceType => "Api";
        public abstract string EntityName { get; }
        protected abstract string EndpointSettingKey { get; }
        protected abstract string DefaultEndpoint { get; }

        public async Task<IReadOnlyCollection<object>> ExtractAsync(CancellationToken cancellationToken = default)
        {
            var baseUrl = _configuration["ApiSettings:BaseUrl"];
            var endpoint = _configuration[$"ApiSettings:{EndpointSettingKey}"] ?? DefaultEndpoint;

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                _logger.LogWarning("No se configuró ApiSettings:BaseUrl. Se omite la extracción API para {Entity}.", EntityName);
                return Array.Empty<object>();
            }

            var records = await _apiClientService.GetAsync<T>(endpoint, cancellationToken);
            _logger.LogInformation("Extracción API completada para {Entity}. Endpoint: {Endpoint}. Registros: {Count}", EntityName, endpoint, records.Count);
            return records.Cast<object>().ToList();
        }
    }
}
