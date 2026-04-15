using Domain.Entities.Csv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extractors.CSV
{
    public class OrderDetailCsvExtractor : CsvExtractorBase<OrderDetailCsv>
    {
        public OrderDetailCsvExtractor(CsvReaderService csv, IConfiguration configuration, ILogger<OrderDetailCsvExtractor> logger)
            : base(csv, configuration, logger)
        {
        }

        public override string SourceName => "SalesCsv";
        public override string EntityName => nameof(OrderDetailCsv);
        protected override string DefaultRelativePath => Path.Combine("Data", "orderdetails.csv");
    }
}
