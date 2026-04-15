namespace Domain.Models
{
    public class ExtractionBatch
    {
        public string SourceName { get; init; } = string.Empty;
        public string SourceType { get; init; } = string.Empty;
        public string EntityName { get; init; } = string.Empty;
        public DateTimeOffset ExtractedAt { get; init; } = DateTimeOffset.UtcNow;
        public IReadOnlyCollection<object> Records { get; init; } = Array.Empty<object>();
        public int RecordCount => Records.Count;
    }
}
