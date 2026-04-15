namespace Api.Data.Entities
{
    public class DataSourceInfo
    {
        public string Name { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public IReadOnlyCollection<string> AvailableEntities { get; init; } = Array.Empty<string>();
    }
}
