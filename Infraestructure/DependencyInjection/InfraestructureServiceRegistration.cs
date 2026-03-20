using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Extractors.API;
using Infrastructure.Extractors.CSV;
using Infrastructure.Staging;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddHttpClient<ApiClientService>();

            services.AddSingleton<CsvReaderService>();

            services.AddTransient<ICsvExtractor, CustomerCsvExtractor>();
            services.AddTransient<ICsvExtractor, ProductCsvExtractor>();

            services.AddTransient<IApiExtractor, CustomerApiExtractor>();
            services.AddTransient<IApiExtractor, ProductApiExtractor>();

            services.AddTransient<IStagingService, StagingService>();
            services.AddTransient<IDataLoader, DataLoader>();

            return services;
        }
    }
}