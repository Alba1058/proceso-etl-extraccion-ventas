using Application.Interfaces;
using Application.Result;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class SalesAnalyticsHandlerService : ISalesAnalyticsHandlerService
    {
        private readonly IETLService _etlService;
        private readonly ILogger<SalesAnalyticsHandlerService> _logger;

        public SalesAnalyticsHandlerService(IETLService etlService, ILogger<SalesAnalyticsHandlerService> logger)
        {
            _etlService = etlService;
            _logger = logger;
        }

        public async Task<ServiceResult> ProcessSalesAnalyticsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _etlService.EjecutarProcesoETLAsync(cancellationToken);
                return ServiceResult.Success("Proceso analitico de ventas ejecutado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando el proceso analitico de ventas.");
                return ServiceResult.Failure($"Error ejecutando el proceso analitico de ventas: {ex.Message}");
            }
        }
    }
}
