using Application.Interfaces;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ETLService : IETLService
    {
        private readonly IEnumerable<IExtractor<object>> _extractores;
        private readonly ITransformationService _transformationService;
        private readonly IDataLoader _dataLoader;
        private readonly IStagingService _stagingService; 
        private readonly ILogger<ETLService> _logger;

        public ETLService(
            IEnumerable<IExtractor<object>> extractores,
            ITransformationService transformationService,
            IDataLoader dataLoader,
            IStagingService stagingService,
            ILogger<ETLService> logger)
        {
            _extractores = extractores;
            _transformationService = transformationService;
            _dataLoader = dataLoader;
            _stagingService = stagingService;
            _logger = logger;
        }

        public async Task EjecutarProcesoETLAsync()
        {
            _logger.LogInformation("--- INICIANDO FASE DE EXTRACCIÓN ---");

            var datosExtraidos = new List<object>();

            foreach (var extractor in _extractores)
            {
                try
                {
                    var data = await extractor.ExtractAsync();

                    if (data != null && data.Any())
                    {
                        datosExtraidos.AddRange(data);
                        string fileName = extractor.GetType().Name;

                        await _stagingService.SaveAsync(fileName, data);
                        _logger.LogInformation("Datos de {Extractor} guardados en Staging exitosamente.", fileName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al extraer datos desde {Extractor}", extractor.GetType().Name);
                }
            }

            _logger.LogInformation("--- EXTRACCIÓN Y STAGING COMPLETADOS ---");

            //fase de Transformación Simulada por ahora
            _logger.LogInformation("--- INICIANDO FASE DE TRANSFORMACIÓN ---");
            await _transformationService.TransformAsync();
            _logger.LogInformation("--- TRANSFORMACIÓN COMPLETADA ---");

            _logger.LogInformation("--- INICIANDO FASE DE CARGA ---");
            await _dataLoader.LoadAsync(datosExtraidos);
            _logger.LogInformation("--- CARGA COMPLETADA ---");

            _logger.LogInformation("=== PROCESO ETL FINALIZADO CON ÉXITO ===");
        }
    }
}