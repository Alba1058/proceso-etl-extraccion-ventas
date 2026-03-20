using Domain.Interfaces;

namespace Infrastructure.Extractors.CSV
{
    public class CustomerCsvExtractor : ICsvExtractor
    {
        private readonly CsvReaderService _csv;

        public CustomerCsvExtractor(CsvReaderService csv)
        {
            _csv = csv;
        }

        public async Task<List<object>> ExtractAsync()
        {
            var data = await _csv.ReadAsync<object>("Data/customers.csv");
            return data.Cast<object>().ToList();
        }
    }
}