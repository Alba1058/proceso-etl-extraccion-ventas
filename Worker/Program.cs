using Application.Interfaces;
using Application.Services;
using Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence.DependencyInjection;

namespace Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            LoadWorkerConfiguration(builder.Configuration);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            var dataWarehouseConnectionString = builder.Configuration.GetConnectionString("DataWarehouseDb")
                ?? throw new InvalidOperationException(
                    "No se configuro la conexion DataWarehouseDb. Verifica Worker/appsettings.json o la ruta del archivo de configuracion.");

            builder.Services.AddTransient<IETLService, ETLService>();
            builder.Services.AddTransient<ISalesAnalyticsHandlerService, SalesAnalyticsHandlerService>();
            builder.Services.AddTransient<ITransformationService, TransformationService>();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddPersistence(dataWarehouseConnectionString);
            builder.Services.AddHostedService<EtlWorker>();

            var host = builder.Build();
            host.Run();
        }

        private static void LoadWorkerConfiguration(ConfigurationManager configuration)
        {
            var candidateDirectories = new[]
            {
                AppContext.BaseDirectory,
                Directory.GetCurrentDirectory(),
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..")),
                Path.Combine(Directory.GetCurrentDirectory(), "Worker")
            }
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(Directory.Exists);

            foreach (var directory in candidateDirectories)
            {
                var appSettingsPath = Path.Combine(directory, "appsettings.json");
                if (!File.Exists(appSettingsPath))
                {
                    continue;
                }

                configuration.AddJsonFile(appSettingsPath, optional: false, reloadOnChange: true);

                var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                    ?? Environments.Production;
                var environmentFile = Path.Combine(directory, $"appsettings.{environmentName}.json");

                if (File.Exists(environmentFile))
                {
                    configuration.AddJsonFile(environmentFile, optional: true, reloadOnChange: true);
                }

                return;
            }
        }
    }
}
