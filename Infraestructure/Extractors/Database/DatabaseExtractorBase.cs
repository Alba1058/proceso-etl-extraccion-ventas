using Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extractors.Database
{
    public abstract class DatabaseExtractorBase<T> : IDatabaseExtractor where T : class
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        protected DatabaseExtractorBase(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public abstract string SourceName { get; }
        public string SourceType => "Database";
        public abstract string EntityName { get; }
        protected abstract string DefaultQuery { get; }
        protected abstract T MapRecord(SqlDataReader reader);

        public async Task<IReadOnlyCollection<object>> ExtractAsync(CancellationToken cancellationToken = default)
        {
            var connectionString = _configuration.GetConnectionString("SourceDb");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                _logger.LogWarning("No se configuró la conexión SourceDb. Se omite la extracción de base de datos para {Entity}.", EntityName);
                return Array.Empty<object>();
            }

            var query = _configuration[$"DatabaseQueries:{EntityName}"] ?? DefaultQuery;
            var result = new List<object>();

            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand(query, conn);
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                result.Add(MapRecord(reader));
            }

            _logger.LogInformation("Extracción DB completada para {Entity}. Consulta: {Query}. Registros: {Count}", EntityName, query, result.Count);
            return result;
        }
    }
}
