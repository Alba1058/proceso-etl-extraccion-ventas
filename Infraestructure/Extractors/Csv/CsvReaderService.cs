using CsvHelper;
using CsvHelper.Configuration;
using Domain.Entities.Db;
using System.Globalization;

namespace Infrastructure.Extractors.CSV
{
    public class CsvReaderService
    {
        public async Task<List<T>> ReadAsync<T>(string path, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(path))
            {
                return new List<T>();
            }

            if (typeof(T) == typeof(OrderDetail))
            {
                var orderDetails = await ReadOrderDetailsAsync(path, cancellationToken);
                return orderDetails.Cast<T>().ToList();
            }

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, BuildConfiguration());

            var records = csv.GetRecords<T>().ToList();
            await Task.CompletedTask;
            return records;
        }

        private static CsvConfiguration BuildConfiguration()
        {
            return new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                PrepareHeaderForMatch = args => args.Header?.Trim() ?? string.Empty,
                IgnoreBlankLines = true
            };
        }

        private static async Task<List<OrderDetail>> ReadOrderDetailsAsync(string path, CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, BuildConfiguration());

            var records = new List<OrderDetail>();

            if (!await csv.ReadAsync() || !csv.ReadHeader())
            {
                return records;
            }

            while (await csv.ReadAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var quantity = TryGetField<int>(csv, "Quantity");
                var unitPrice = TryGetField<decimal>(csv, "UnitPrice");
                var totalPrice = TryGetField<decimal>(csv, "TotalPrice");

                if (unitPrice <= 0 && quantity > 0 && totalPrice > 0)
                {
                    unitPrice = decimal.Round(totalPrice / quantity, 2, MidpointRounding.AwayFromZero);
                }

                records.Add(new OrderDetail
                {
                    OrderID = TryGetField<int>(csv, "OrderID"),
                    ProductID = TryGetField<int>(csv, "ProductID"),
                    Quantity = quantity,
                    UnitPrice = unitPrice
                });
            }

            return records;
        }

        private static TField TryGetField<TField>(CsvReader csv, string headerName)
        {
            try
            {
                var field = csv.GetField<TField>(headerName);
                if (field is null)
                {
                    return typeof(TField).IsValueType ? Activator.CreateInstance<TField>() : default!;
                }

                return field;
            }
            catch
            {
                return typeof(TField).IsValueType ? Activator.CreateInstance<TField>() : default!;
            }
        }
    }
}
