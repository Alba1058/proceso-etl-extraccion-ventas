using Domain.Interfaces;

namespace Infrastructure.Data
{
    public class DataLoader : IDataLoader
    {
        public async Task LoadAsync(IEnumerable<object> data)
        {
          
            await Task.Delay(100);
        }
    }
}