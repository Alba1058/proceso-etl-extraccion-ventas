using Domain.Entities.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extractors.API
{
    public class ProductApiExtractor : ApiExtractorBase<ProductApiDto>
    {
        public ProductApiExtractor(ApiClientService apiClient, IConfiguration configuration, ILogger<ProductApiExtractor> logger)
            : base(apiClient, configuration, logger)
        {
        }

        public override string SourceName => "SalesApi";
        public override string EntityName => nameof(ProductApiDto);
        protected override string EndpointSettingKey => "ProductsEndpoint";
        protected override string DefaultEndpoint => "products";
    }
}
