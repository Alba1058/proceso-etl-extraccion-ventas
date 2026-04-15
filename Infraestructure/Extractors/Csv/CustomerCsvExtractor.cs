using Domain.Entities.Csv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extractors.CSV
{
    public class CustomerCsvExtractor : CsvExtractorBase<CustomerCsv>
    {
        public CustomerCsvExtractor(CsvReaderService csv, IConfiguration configuration, ILogger<CustomerCsvExtractor> logger)
            : base(csv, configuration, logger)
        {
        }

        public override string SourceName => "SalesCsv";
        public override string EntityName => nameof(CustomerCsv);
        protected override string DefaultRelativePath => Path.Combine("Data", "customers.csv");
    }
}
