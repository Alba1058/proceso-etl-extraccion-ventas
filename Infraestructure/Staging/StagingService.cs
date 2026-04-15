using Domain.Interfaces;
using Domain.Models;
using Infrastructure.IO;
using System.Text.Json;

namespace Infrastructure.Staging
{
    public class StagingService : IStagingService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        public async Task SaveRawAsync(ExtractionBatch batch, CancellationToken cancellationToken = default)
        {
            var baseDirectory = PathResolver.ResolveDirectory(Path.Combine("StagingArea", "raw"));
            var fileName = $"{batch.SourceType}_{batch.SourceName}_{batch.EntityName}.json";
            var path = Path.Combine(baseDirectory, fileName);
            var json = JsonSerializer.Serialize(batch, JsonOptions);

            await File.WriteAllTextAsync(path, json, cancellationToken);
        }

        public async Task SavePreparedAsync(PreparedSalesData data, CancellationToken cancellationToken = default)
        {
            var baseDirectory = PathResolver.ResolveDirectory(Path.Combine("StagingArea", "prepared"));
            var path = Path.Combine(baseDirectory, "sales-prepared.json");
            var json = JsonSerializer.Serialize(data, JsonOptions);

            await File.WriteAllTextAsync(path, json, cancellationToken);
        }
    }
}
