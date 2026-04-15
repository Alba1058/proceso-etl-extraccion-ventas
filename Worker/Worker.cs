using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Worker
{
    public class EtlWorker : BackgroundService
    {
        private readonly ILogger<EtlWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _applicationLifetime;

        public EtlWorker(
            ILogger<EtlWorker> logger,
            IServiceProvider serviceProvider,
            IHostApplicationLifetime applicationLifetime)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _applicationLifetime = applicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio Worker iniciado a las: {time}", DateTimeOffset.Now);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var salesAnalyticsHandlerService = scope.ServiceProvider.GetRequiredService<ISalesAnalyticsHandlerService>();

                _logger.LogInformation("Arrancando ciclo del proceso analitico de ventas...");

                var stopwatch = Stopwatch.StartNew();
                
                var result = await salesAnalyticsHandlerService.ProcessSalesAnalyticsAsync(stoppingToken);

                if (!result.IsSuccess)
                {
                    throw new InvalidOperationException(result.Message);
                }

                stopwatch.Stop();

                _logger.LogInformation("Ciclo ETL finalizado exitosamente. Tiempo total de ejecucion: {ms} ms", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "El proceso ETL falló críticamente.");
            }
            finally
            {
                _applicationLifetime.StopApplication();
            }
        }
    }
}
