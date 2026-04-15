using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Result;
using SistemaAnalisisVentas.Domain.Entities.Dwh.Dimensions;
using SistemaAnalisisVentas.Domain.Entities.Dwh.Facts;

namespace Application.Repositories
{
    public interface IDwhRepository
    {
        Task LoadCustomersAsync(IEnumerable<DimCliente> customers);
        Task LoadProductsAsync(IEnumerable<DimProducto> products);
        Task LoadTimeAsync(IEnumerable<DimTiempo> times);
        Task LoadBranchesAsync(IEnumerable<DimSucursal> branches);
        Task LoadLocationsAsync(IEnumerable<DimUbicacion> locations);
        Task LoadDataSourcesAsync(IEnumerable<DimFuenteDatos> sources);
        Task LoadSalesAsync(IEnumerable<FactVenta> salesSales);
        Task<Application.Result.ServiceResult> LoadAnalyticsDataAsync(object preparedData, System.Threading.CancellationToken cancellationToken);
    }
}
