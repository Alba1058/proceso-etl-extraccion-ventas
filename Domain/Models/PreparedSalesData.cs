using Domain.Entities.Csv;

namespace Domain.Models
{
    public class PreparedSalesData
    {
        public List<CustomerCsv> Customers { get; init; } = new();
        public List<ProductCsv> Products { get; init; } = new();
        public List<OrderCsv> Orders { get; init; } = new();
        public List<OrderDetailCsv> OrderDetails { get; init; } = new();
        public List<PreparedSourceRecord> Sources { get; init; } = new();
        public List<PreparedSaleFact> SalesFacts { get; init; } = new();
        public DateTimeOffset PreparedAt { get; init; } = DateTimeOffset.UtcNow;

        public object ToSummary() => new
        {
            PreparedAt,
            Customers = Customers.Count,
            Products = Products.Count,
            Orders = Orders.Count,
            OrderDetails = OrderDetails.Count,
            Sources = Sources.Count,
            SalesFacts = SalesFacts.Count
        };
    }
}
