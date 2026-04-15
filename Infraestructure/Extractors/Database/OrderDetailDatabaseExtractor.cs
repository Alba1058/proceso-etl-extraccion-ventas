using Domain.Entities.Db;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extractors.Database
{
    public class OrderDetailDatabaseExtractor : DatabaseExtractorBase<OrderDetail>
    {
        public OrderDetailDatabaseExtractor(IConfiguration configuration, ILogger<OrderDetailDatabaseExtractor> logger)
            : base(configuration, logger)
        {
        }

        public override string SourceName => "VentasOrigen";
        public override string EntityName => nameof(OrderDetail);
        protected override string DefaultQuery => "SELECT OrderID, ProductID, Quantity, UnitPrice FROM OrderDetails";

        protected override OrderDetail MapRecord(SqlDataReader reader)
        {
            return new OrderDetail
            {
                OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice"))
            };
        }
    }
}
