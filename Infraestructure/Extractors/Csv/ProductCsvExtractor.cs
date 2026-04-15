using Domain.Entities.Csv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extractors.CSV
{
    public class ProductCsvExtractor : CsvExtractorBase<ProductCsv>
    {
        public ProductCsvExtractor(CsvReaderService csv, IConfiguration configuration, ILogger<ProductCsvExtractor> logger)
            : base(csv, configuration, logger)
        {
        }

        public override string SourceName => "SalesCsv";
        public override string EntityName => nameof(ProductCsv);
        protected override string DefaultRelativePath => Path.Combine("Data", "products.csv");
    }
}
