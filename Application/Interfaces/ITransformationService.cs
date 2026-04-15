using Domain.Models;

namespace Application.Interfaces
{
    public interface ITransformationService
    {
        Task<PreparedSalesData> TransformAsync(IEnumerable<ExtractionBatch> batches, CancellationToken cancellationToken = default);
    }
}
