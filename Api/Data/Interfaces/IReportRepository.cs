using Api.Data.Entities;

namespace Api.Data.Interfaces
{
    public interface IReportRepository
    {
        Task<IEnumerable<ReportRowDto>> GetVentasPorProductoAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ReportRowDto>> GetVentasPorClienteAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<SalesByMonthDto>> GetVentasPorMesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TopProductDto>> GetTopProductosAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TopClientDto>> GetTopClientesAsync(CancellationToken cancellationToken = default);
    }
}
