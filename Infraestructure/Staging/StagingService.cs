using Domain.Interfaces;
using System.Text.Json;

namespace Infrastructure.Staging
{
    public class StagingService : IStagingService
    {
        public async Task SaveAsync<T>(string key, List<T> data)
        {
            var json = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync($"staging_{key}.json", json);
        }

        public async Task<List<T>> LoadAsync<T>(string key)
        {
            var path = $"staging_{key}.json";

            if (!File.Exists(path))
                return new List<T>();

            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
    }
}