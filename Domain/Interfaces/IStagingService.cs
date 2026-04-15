using Domain.Models;

namespace Domain.Interfaces
{
    public interface IStagingService
    {
        Task SaveRawAsync(ExtractionBatch batch, CancellationToken cancellationToken = default);
        Task SavePreparedAsync(PreparedSalesData data, CancellationToken cancellationToken = default);
    }
}
