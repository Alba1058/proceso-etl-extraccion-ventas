using Domain.Entities.Db;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extractors.Database
{
    public class OrderDatabaseExtractor : DatabaseExtractorBase<Order>
    {
        public OrderDatabaseExtractor(IConfiguration configuration, ILogger<OrderDatabaseExtractor> logger)
            : base(configuration, logger)
        {
        }

        public override string SourceName => "VentasOrigen";
        public override string EntityName => nameof(Order);
        protected override string DefaultQuery => "SELECT OrderID, OrderDate, CustomerID FROM Orders";

        protected override Order MapRecord(SqlDataReader reader)
        {
            return new Order
            {
                OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID"))
            };
        }
    }
}
