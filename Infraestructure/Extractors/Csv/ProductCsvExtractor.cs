using Domain.Interfaces;
using Infrastructure.Extractors.CSV;

namespace Infrastructure.Extractors.CSV
{
    public class ProductCsvExtractor : ICsvExtractor
    {
        private readonly CsvReaderService _csv;

        public ProductCsvExtractor(CsvReaderService csv)
        {
            _csv = csv;
        }

        public async Task<List<object>> ExtractAsync()
        {
            var data = await _csv.ReadAsync<object>("Data/products.csv");
            return data.Cast<object>().ToList();
        }
    }
}