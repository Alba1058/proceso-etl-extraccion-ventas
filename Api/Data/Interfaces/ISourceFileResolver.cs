namespace Api.Data.Interfaces
{
    public interface ISourceFileResolver
    {
        string ResolveRequiredPath(string key, string fallbackPath);
    }
}
