using Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Worker
{
    public class EtlWorker : BackgroundService
    {
        private readonly ILogger<EtlWorker> _logger;
        private readonly IETLService _etlService; 

        public EtlWorker(ILogger<EtlWorker> logger, IETLService etlService)
        {
            _logger = logger;
            _etlService = etlService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio Worker iniciado a las: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Arrancando ciclo del ETL...");

                var stopwatch = Stopwatch.StartNew();

                try
                {
                    await _etlService.EjecutarProcesoETLAsync();

                    stopwatch.Stop();
                    _logger.LogInformation("Ciclo ETL finalizado exitosamente. Tiempo total de ejecución: {ms} ms", stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _logger.LogCritical(ex, "El proceso ETL falló críticamente después de {ms} ms.", stopwatch.ElapsedMilliseconds);
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}