using Domain.Models;

namespace Domain.Interfaces
{
    public interface IDataLoader
    {
        Task LoadAsync(PreparedSalesData data, CancellationToken cancellationToken = default);
    }
}
