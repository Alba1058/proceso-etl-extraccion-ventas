using Domain.Interfaces;
using Infrastructure.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extractors.CSV
{
    public abstract class CsvExtractorBase<T> : ICsvExtractor where T : class
    {
        private readonly CsvReaderService _csvReaderService;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        protected CsvExtractorBase(CsvReaderService csvReaderService, IConfiguration configuration, ILogger logger)
        {
            _csvReaderService = csvReaderService;
            _configuration = configuration;
            _logger = logger;
        }

        public abstract string SourceName { get; }
        public string SourceType => "Csv";
        public abstract string EntityName { get; }
        protected abstract string DefaultRelativePath { get; }

        protected virtual string ConfigKey => EntityName;

        public async Task<IReadOnlyCollection<object>> ExtractAsync(CancellationToken cancellationToken = default)
        {
            var configuredPath = _configuration[$"SourceFiles:{ConfigKey}"] ?? DefaultRelativePath;
            var resolvedPath = PathResolver.ResolveExistingPath(configuredPath);

            if (!File.Exists(resolvedPath))
            {
                _logger.LogWarning("No se encontró el archivo CSV configurado para {Entity}: {Path}", EntityName, resolvedPath);
                return Array.Empty<object>();
            }

            var records = await _csvReaderService.ReadAsync<T>(resolvedPath, cancellationToken);
            _logger.LogInformation("Extracción CSV completada para {Entity}. Archivo: {Path}. Registros: {Count}", EntityName, resolvedPath, records.Count);
            return records.Cast<object>().ToList();
        }
    }
}
