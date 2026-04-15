using Domain.Entities.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extractors.API
{
    public class CustomerApiExtractor : ApiExtractorBase<CustomerApiDto>
    {
        public CustomerApiExtractor(ApiClientService apiClient, IConfiguration configuration, ILogger<CustomerApiExtractor> logger)
            : base(apiClient, configuration, logger)
        {
        }

        public override string SourceName => "SalesApi";
        public override string EntityName => nameof(CustomerApiDto);
        protected override string EndpointSettingKey => "CustomersEndpoint";
        protected override string DefaultEndpoint => "customers";
    }
}
