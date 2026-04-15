using Domain.Interfaces;
using Infrastructure.Extractors.API;
using Infrastructure.Extractors.CSV;
using Infrastructure.Extractors.Database;
using Infrastructure.Staging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var enableApiExtractors = configuration.GetValue<bool>("ExtractionSources:EnableApi");
            var enableDatabaseExtractors = configuration.GetValue<bool>("ExtractionSources:EnableDatabase");

            services.AddHttpClient<ApiClientService>((_, client) =>
            {
                var baseUrl = configuration["ApiSettings:BaseUrl"];
                if (!string.IsNullOrWhiteSpace(baseUrl))
                {
                    client.BaseAddress = new Uri(baseUrl);
                }

                var apiKey = configuration["ApiSettings:ApiKey"];
                if (!string.IsNullOrWhiteSpace(apiKey))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                }
            });

            services.AddSingleton<CsvReaderService>();

            services.AddTransient<IExtractor<object>, CustomerCsvExtractor>();
            services.AddTransient<IExtractor<object>, ProductCsvExtractor>();
            services.AddTransient<IExtractor<object>, OrderCsvExtractor>();
            services.AddTransient<IExtractor<object>, OrderDetailCsvExtractor>();

            if (enableApiExtractors)
            {
                services.AddTransient<IExtractor<object>, CustomerApiExtractor>();
                services.AddTransient<IExtractor<object>, ProductApiExtractor>();
            }

            if (enableDatabaseExtractors)
            {
                services.AddTransient<IExtractor<object>, OrderDatabaseExtractor>();
                services.AddTransient<IExtractor<object>, OrderDetailDatabaseExtractor>();
            }

            services.AddTransient<IStagingService, StagingService>();

            return services;
        }
    }
}
