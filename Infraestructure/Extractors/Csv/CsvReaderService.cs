using CsvHelper;
using System.Formats.Asn1;
using System.Globalization;

namespace Infrastructure.Extractors.CSV
{
    public class CsvReaderService
    {
        public async Task<List<T>> ReadAsync<T>(string path)
        {
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<T>().ToList();
            return await Task.FromResult(records);
        }
    }
}