using Api.Data.Entities;
using Domain.Entities.Api;

namespace Api.Data.Interfaces
{
    public interface IVentasSourceRepository
    {
        Task<IEnumerable<VentaExtractDto>> GetVentasParaEtlAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<DataSourceInfo>> GetAvailableSourcesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<SalesSourceSummary>> GetSourceSummaryAsync(CancellationToken cancellationToken = default);
    }
}
