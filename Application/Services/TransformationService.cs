using Application.Interfaces;
using Domain.Entities.Csv;
using Domain.Entities.Db;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class TransformationService : ITransformationService
    {
        private const string DefaultBranchName = "Sucursal Principal";
        private const string DefaultBranchAddress = "No especificada";
        private const string DefaultRegion = "Region no especificada";

        private readonly ILogger<TransformationService> _logger;

        public TransformationService(ILogger<TransformationService> logger)
        {
            _logger = logger;
        }

        public Task<PreparedSalesData> TransformAsync(IEnumerable<ExtractionBatch> batches, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Iniciando transformacion y depuracion de datos...");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var batchList = batches.ToList();

                var customers = BuildCustomers(batchList);
                var products = BuildProducts(batchList);
                var validCustomerIds = customers.Select(customer => customer.CustomerID).ToHashSet();
                var validProductIds = products.Select(product => product.ProductID).ToHashSet();
                var orders = BuildOrders(batchList, validCustomerIds);
                var validOrderIds = orders.Select(order => order.OrderID).ToHashSet();
                var orderDetails = BuildOrderDetails(batchList, validOrderIds, validProductIds);
                var sources = BuildSources(batchList);
                var salesFacts = BuildSalesFacts(batchList, customers, products, orders);

                var preparedData = new PreparedSalesData
                {
                    Customers = customers,
                    Products = products,
                    Orders = orders,
                    OrderDetails = orderDetails,
                    Sources = sources,
                    SalesFacts = salesFacts,
                    PreparedAt = DateTimeOffset.UtcNow
                };

                _logger.LogInformation(
                    "Transformacion completada. Clientes: {Customers}, Productos: {Products}, Ordenes: {Orders}, Detalles: {Details}, Fuentes: {Sources}, Ventas: {SalesFacts}",
                    customers.Count,
                    products.Count,
                    orders.Count,
                    orderDetails.Count,
                    sources.Count,
                    salesFacts.Count);

                return Task.FromResult(preparedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la transformacion.");
                throw;
            }
        }

        private static List<CustomerCsv> BuildCustomers(IEnumerable<ExtractionBatch> batches)
        {
            return GetRecords<CustomerCsv>(batches, nameof(CustomerCsv))
                .Where(customer => customer.CustomerID > 0)
                .Select(customer => new CustomerCsv
                {
                    CustomerID = customer.CustomerID,
                    FirstName = NormalizeText(customer.FirstName),
                    LastName = NormalizeText(customer.LastName),
                    Email = NormalizeText(customer.Email).ToLowerInvariant(),
                    Phone = NormalizeText(customer.Phone),
                    City = NormalizeText(customer.City),
                    Country = NormalizeText(customer.Country)
                })
                .GroupBy(customer => customer.CustomerID)
                .Select(group => group.First())
                .ToList();
        }

        private static List<ProductCsv> BuildProducts(IEnumerable<ExtractionBatch> batches)
        {
            return GetRecords<ProductCsv>(batches, nameof(ProductCsv))
                .Where(product => product.ProductID > 0)
                .Select(product => new ProductCsv
                {
                    ProductID = product.ProductID,
                    ProductName = NormalizeText(product.ProductName),
                    Category = NormalizeText(product.Category),
                    Price = decimal.Round(product.Price < 0 ? 0 : product.Price, 2)
                })
                .GroupBy(product => product.ProductID)
                .Select(group => group.First())
                .ToList();
        }

        private static List<OrderCsv> BuildOrders(IEnumerable<ExtractionBatch> batches, ISet<int> validCustomerIds)
        {
            return GetRecords<OrderCsv>(batches, nameof(OrderCsv))
                .Where(order => order.OrderID > 0 && validCustomerIds.Contains(order.CustomerID))
                .Select(NormalizeOrder)
                .GroupBy(order => order.OrderID)
                .Select(group => group.First())
                .ToList();
        }

        private static List<OrderDetailCsv> BuildOrderDetails(IEnumerable<ExtractionBatch> batches, ISet<int> validOrderIds, ISet<int> validProductIds)
        {
            return GetRecords<OrderDetailCsv>(batches, nameof(OrderDetailCsv))
                .Where(detail => validOrderIds.Contains(detail.OrderID) && validProductIds.Contains(detail.ProductID))
                .Select(detail => new OrderDetailCsv
                {
                    OrderID = detail.OrderID,
                    ProductID = detail.ProductID,
                    Quantity = Math.Max(detail.Quantity, 0),
                    UnitPrice = decimal.Round(Math.Max(detail.UnitPrice, 0), 2)
                })
                .GroupBy(detail => new { detail.OrderID, detail.ProductID })
                .Select(group => group.First())
                .ToList();
        }

        private static List<PreparedSourceRecord> BuildSources(IEnumerable<ExtractionBatch> batches)
        {
            return batches
                .Select(batch => new PreparedSourceRecord
                {
                    SourceName = batch.SourceName,
                    SourceType = batch.SourceType,
                    Description = $"Datos extraidos desde {batch.SourceType} para la entidad {batch.EntityName}"
                })
                .GroupBy(source => new { source.SourceName, source.SourceType })
                .Select(group => group.First())
                .ToList();
        }

        private static List<PreparedSaleFact> BuildSalesFacts(
            IEnumerable<ExtractionBatch> batches,
            IReadOnlyCollection<CustomerCsv> customers,
            IReadOnlyCollection<ProductCsv> products,
            IReadOnlyCollection<OrderCsv> orders)
        {
            var validCustomerIds = customers.Select(customer => customer.CustomerID).ToHashSet();
            var validProductIds = products.Select(product => product.ProductID).ToHashSet();
            var customerLookup = customers.ToDictionary(customer => customer.CustomerID);
            var orderLookupBySource = BuildOrderLookup(batches, validCustomerIds);
            var fallbackOrderLookup = orders.ToDictionary(order => order.OrderID);

            return batches
                .Where(batch => batch.EntityName.Equals(nameof(OrderDetailCsv), StringComparison.OrdinalIgnoreCase))
                .SelectMany(batch => batch.Records.OfType<OrderDetailCsv>(), (batch, detail) => new { batch, detail })
                .Where(item => validProductIds.Contains(item.detail.ProductID))
                .Select(item => CreatePreparedFact(item.batch, item.detail, orderLookupBySource, fallbackOrderLookup, customerLookup))
                .Where(fact => fact is not null)
                .Select(fact => fact!)
                .GroupBy(fact => new
                {
                    fact.OrderNumber,
                    fact.ProductSourceId,
                    fact.CustomerSourceId,
                    fact.SourceName,
                    fact.SourceType
                })
                .Select(group => group.First())
                .ToList();
        }

        private static IEnumerable<T> GetRecords<T>(IEnumerable<ExtractionBatch> batches, string entityName)
        {
            return batches
                .Where(batch => batch.EntityName.Equals(entityName, StringComparison.OrdinalIgnoreCase))
                .SelectMany(batch => batch.Records.OfType<T>());
        }

        private static OrderCsv NormalizeOrder(OrderCsv order)
        {
            return new OrderCsv
            {
                OrderID = order.OrderID,
                CustomerID = order.CustomerID,
                OrderDate = order.OrderDate == default ? DateTime.UtcNow.Date : order.OrderDate.Date
            };
        }

        private static Dictionary<(string SourceType, string SourceName, int OrderId), OrderCsv> BuildOrderLookup(
            IEnumerable<ExtractionBatch> batches,
            HashSet<int> validCustomerIds)
        {
            return batches
                .Where(batch => batch.EntityName.Equals(nameof(OrderCsv), StringComparison.OrdinalIgnoreCase))
                .SelectMany(batch => batch.Records.OfType<OrderCsv>(), (batch, order) => new { batch, order })
                .Where(item => item.order.OrderID > 0 && validCustomerIds.Contains(item.order.CustomerID))
                .GroupBy(item => (item.batch.SourceType, item.batch.SourceName, item.order.OrderID))
                .ToDictionary(
                    group => group.Key,
                    group => NormalizeOrder(group.First().order));
        }

        private static PreparedSaleFact? CreatePreparedFact(
            ExtractionBatch batch,
            OrderDetailCsv detail,
            IReadOnlyDictionary<(string SourceType, string SourceName, int OrderId), OrderCsv> orderLookupBySource,
            IReadOnlyDictionary<int, OrderCsv> fallbackOrderLookup,
            IReadOnlyDictionary<int, CustomerCsv> customerLookup)
        {
            if (detail.Quantity <= 0 || detail.UnitPrice < 0)
            {
                return null;
            }

            if (!orderLookupBySource.TryGetValue((batch.SourceType, batch.SourceName, detail.OrderID), out var order)
                && !fallbackOrderLookup.TryGetValue(detail.OrderID, out order))
            {
                return null;
            }

            if (!customerLookup.TryGetValue(order.CustomerID, out var customer))
            {
                return null;
            }

            return new PreparedSaleFact
            {
                OrderNumber = order.OrderID,
                ProductSourceId = detail.ProductID,
                CustomerSourceId = order.CustomerID,
                OrderDate = order.OrderDate,
                Quantity = detail.Quantity,
                UnitPrice = decimal.Round(detail.UnitPrice, 2),
                TotalSale = decimal.Round(detail.TotalPrice, 2),
                SourceName = batch.SourceName,
                SourceType = batch.SourceType,
                BranchName = DefaultBranchName,
                BranchAddress = DefaultBranchAddress,
                Country = NormalizeText(customer.Country),
                Region = DefaultRegion,
                City = NormalizeText(customer.City)
            };
        }

        private static string NormalizeText(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? "N/A" : value.Trim();
        }
    }
}
