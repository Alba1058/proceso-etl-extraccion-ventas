using Domain.Interfaces;
using Infrastructure.Extractors.CSV;

namespace Infrastructure.Extractors.CSV
{
    public class OrderCsvExtractor : ICsvExtractor
    {
        private readonly CsvReaderService _csv;

        public OrderCsvExtractor(CsvReaderService csv)
        {
            _csv = csv;
        }

        public async Task<List<object>> ExtractAsync()
        {
            var data = await _csv.ReadAsync<object>("Data/orders.csv");
            return data.Cast<object>().ToList();
        }
    }
}