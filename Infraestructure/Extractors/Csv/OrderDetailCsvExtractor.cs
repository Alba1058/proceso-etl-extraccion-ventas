using Domain.Interfaces;
using Infrastructure.Extractors.CSV;

namespace Infrastructure.Extractors.CSV
{
    public class OrderDetailCsvExtractor : ICsvExtractor
    {
        private readonly CsvReaderService _csv;

        public OrderDetailCsvExtractor(CsvReaderService csv)
        {
            _csv = csv;
        }

        public async Task<List<object>> ExtractAsync()
        {
            var data = await _csv.ReadAsync<object>("Data/orderdetails.csv");
            return data.Cast<object>().ToList();
        }
    }
}