using Application.Interfaces;
using Application.Repositories;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ETLService : IETLService
    {
        private readonly IEnumerable<IExtractor<object>> _extractors;
        private readonly ITransformationService _transformationService;
        private readonly IDwhRepository _dwhRepository;
        private readonly ILogger<ETLService> _logger;

        public ETLService(
            IEnumerable<IExtractor<object>> extractors,
            ITransformationService transformationService,
            IDwhRepository dwhRepository,
            ILogger<ETLService> logger)
        {
            _extractors = extractors;
            _transformationService = transformationService;
            _dwhRepository = dwhRepository;
            _logger = logger;
        }

        public async Task EjecutarProcesoETLAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("--- INICIANDO FASE DE EXTRACCION ---");

            var batches = await ExtractBatchesAsync(cancellationToken);

            _logger.LogInformation("--- EXTRACCION Y STAGING COMPLETADOS ---");

            _logger.LogInformation("--- INICIANDO FASE DE TRANSFORMACION ---");
            var preparedData = await _transformationService.TransformAsync(batches, cancellationToken);
            _logger.LogInformation("--- TRANSFORMACION COMPLETADA ---");

            _logger.LogInformation("--- INICIANDO FASE DE CARGA AL DATA WAREHOUSE ---");
            var result = await _dwhRepository.LoadAnalyticsDataAsync(preparedData, cancellationToken);
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException(result.Message);
            }

            _logger.LogInformation("--- CARGA AL DATA WAREHOUSE COMPLETADA ---");
            _logger.LogInformation("=== PROCESO ETL FINALIZADO CON EXITO ===");
        }

        private async Task<List<ExtractionBatch>> ExtractBatchesAsync(CancellationToken cancellationToken)
        {
            var batches = new List<ExtractionBatch>();

            foreach (var extractor in _extractors)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var data = await extractor.ExtractAsync(cancellationToken);
                    if (data.Count == 0)
                    {
                        LogExtractorWithoutData(extractor);
                        continue;
                    }

                    var batch = BuildBatch(extractor, data);
                    batches.Add(batch);

                    LogBatchSaved(batch);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al extraer datos desde {Extractor}", extractor.GetType().Name);
                }
            }

            return batches;
        }

        private static ExtractionBatch BuildBatch(IExtractor<object> extractor, IReadOnlyCollection<object> data)
        {
            return new ExtractionBatch
            {
                EntityName = extractor.EntityName,
                SourceName = extractor.SourceName,
                SourceType = extractor.SourceType,
                Records = data.ToList(),
                ExtractedAt = DateTimeOffset.UtcNow
            };
        }

        private void LogExtractorWithoutData(IExtractor<object> extractor)
        {
            _logger.LogWarning(
                "El extractor {Extractor} no devolvio datos. Fuente: {SourceType}::{SourceName}, entidad: {Entity}",
                extractor.GetType().Name,
                extractor.SourceType,
                extractor.SourceName,
                extractor.EntityName);
        }

        private void LogBatchSaved(ExtractionBatch batch)
        {
            _logger.LogInformation(
                "Datos guardados en staging para {SourceType}::{SourceName} - {Entity}. Registros: {Count}",
                batch.SourceType,
                batch.SourceName,
                batch.EntityName,
                batch.RecordCount);
        }
    }
}
