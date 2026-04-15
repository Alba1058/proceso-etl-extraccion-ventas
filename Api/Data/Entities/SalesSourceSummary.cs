namespace Api.Data.Entities
{
    public class SalesSourceSummary
    {
        public string EntityName { get; init; } = string.Empty;
        public string SourceName { get; init; } = string.Empty;
        public string SourceType { get; init; } = string.Empty;
        public int RecordCount { get; init; }
    }
}
