namespace Domain.Interfaces
{
    public interface IExtractor<T>
    {
        string SourceName { get; }
        string SourceType { get; }
        string EntityName { get; }
        Task<IReadOnlyCollection<T>> ExtractAsync(CancellationToken cancellationToken = default);
    }
}
