using Domain.Entities.Csv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extractors.CSV
{
    public class OrderCsvExtractor : CsvExtractorBase<OrderCsv>
    {
        public OrderCsvExtractor(CsvReaderService csv, IConfiguration configuration, ILogger<OrderCsvExtractor> logger)
            : base(csv, configuration, logger)
        {
        }

        public override string SourceName => "SalesCsv";
        public override string EntityName => nameof(OrderCsv);
        protected override string DefaultRelativePath => Path.Combine("Data", "orders.csv");
    }
}
