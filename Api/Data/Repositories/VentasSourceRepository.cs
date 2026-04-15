using Api.Data.Entities;
using Api.Data.Interfaces;
using Domain.Entities.Api;
using Domain.Entities.Csv;
using Infrastructure.Extractors.CSV;

namespace Api.Data.Repositories
{
    public class VentasSourceRepository : IVentasSourceRepository
    {
        private readonly CsvReaderService _csvReaderService;
        private readonly ISourceFileResolver _sourceFileResolver;

        public VentasSourceRepository(CsvReaderService csvReaderService, ISourceFileResolver sourceFileResolver)
        {
            _csvReaderService = csvReaderService;
            _sourceFileResolver = sourceFileResolver;
        }

        public async Task<IEnumerable<VentaExtractDto>> GetVentasParaEtlAsync(CancellationToken cancellationToken = default)
        {
            var customers = (await _csvReaderService.ReadAsync<CustomerApiDto>(_sourceFileResolver.ResolveRequiredPath("CustomerCsv", Path.Combine("Data", "customers.csv")), cancellationToken))
                .ToDictionary(x => x.CustomerID);
            var products = (await _csvReaderService.ReadAsync<ProductApiDto>(_sourceFileResolver.ResolveRequiredPath("ProductCsv", Path.Combine("Data", "products.csv")), cancellationToken))
                .ToDictionary(x => x.ProductID);
            var orders = (await _csvReaderService.ReadAsync<OrderCsv>(_sourceFileResolver.ResolveRequiredPath("OrderCsv", Path.Combine("Data", "orders.csv")), cancellationToken))
                .ToDictionary(x => x.OrderID);
            var details = await _csvReaderService.ReadAsync<OrderDetailCsv>(_sourceFileResolver.ResolveRequiredPath("OrderDetailCsv", Path.Combine("Data", "orderdetails.csv")), cancellationToken);

            var ventas = details
                .Where(detail => orders.ContainsKey(detail.OrderID) && products.ContainsKey(detail.ProductID))
                .Select(detail =>
                {
                    var order = orders[detail.OrderID];
                    customers.TryGetValue(order.CustomerID, out var customer);
                    var product = products[detail.ProductID];

                    return new VentaExtractDto
                    {
                        OrderID = order.OrderID,
                        CustomerID = order.CustomerID,
                        FirstName = customer?.FirstName ?? "N/A",
                        LastName = customer?.LastName ?? "N/A",
                        CityName = customer?.City ?? "N/A",
                        CountryName = customer?.Country ?? "N/A",
                        OrderDate = order.OrderDate,
                        ProductID = product.ProductID,
                        ProductName = product.ProductName,
                        CategoryName = product.Category,
                        Quantity = detail.Quantity,
                        Price = detail.UnitPrice > 0 ? detail.UnitPrice : product.Price,
                        TotalPrice = detail.TotalPrice,
                        SourceName = "SalesCsv",
                        SourceType = "Csv"
                    };
                })
                .ToList();

            return ventas;
        }

        public Task<IEnumerable<DataSourceInfo>> GetAvailableSourcesAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<DataSourceInfo> sources =
            [
                new DataSourceInfo
                {
                    Name = "SalesCsv",
                    Type = "Csv",
                    Description = "Archivos CSV locales con clientes, productos, ordenes y detalles de orden.",
                    AvailableEntities = ["Customer", "Product", "Order", "OrderDetail"]
                },
                new DataSourceInfo
                {
                    Name = "SalesApi",
                    Type = "Api",
                    Description = "API local preparada para exponer datos maestros de ventas.",
                    AvailableEntities = ["Customer", "Product", "VentasExtract"]
                },
                new DataSourceInfo
                {
                    Name = "VentasOrigen",
                    Type = "Database",
                    Description = "Base de datos historica o transaccional usada en el ETL.",
                    AvailableEntities = ["Order", "OrderDetail"]
                }
            ];

            return Task.FromResult(sources);
        }

        public async Task<IEnumerable<SalesSourceSummary>> GetSourceSummaryAsync(CancellationToken cancellationToken = default)
        {
            var customers = await _csvReaderService.ReadAsync<CustomerApiDto>(_sourceFileResolver.ResolveRequiredPath("CustomerCsv", Path.Combine("Data", "customers.csv")), cancellationToken);
            var products = await _csvReaderService.ReadAsync<ProductApiDto>(_sourceFileResolver.ResolveRequiredPath("ProductCsv", Path.Combine("Data", "products.csv")), cancellationToken);
            var orders = await _csvReaderService.ReadAsync<OrderCsv>(_sourceFileResolver.ResolveRequiredPath("OrderCsv", Path.Combine("Data", "orders.csv")), cancellationToken);
            var details = await _csvReaderService.ReadAsync<OrderDetailCsv>(_sourceFileResolver.ResolveRequiredPath("OrderDetailCsv", Path.Combine("Data", "orderdetails.csv")), cancellationToken);

            IEnumerable<SalesSourceSummary> summary =
            [
                new SalesSourceSummary { EntityName = "Customer", SourceName = "SalesCsv", SourceType = "Csv", RecordCount = customers.Count },
                new SalesSourceSummary { EntityName = "Product", SourceName = "SalesCsv", SourceType = "Csv", RecordCount = products.Count },
                new SalesSourceSummary { EntityName = "Order", SourceName = "SalesCsv", SourceType = "Csv", RecordCount = orders.Count },
                new SalesSourceSummary { EntityName = "OrderDetail", SourceName = "SalesCsv", SourceType = "Csv", RecordCount = details.Count }
            ];

            return summary;
        }
    }
}
