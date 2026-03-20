using Domain.Interfaces;
using Microsoft.Data.SqlClient;

namespace Infrastructure.Extractors.Database
{
    public class OrderDatabaseExtractor : IDatabaseExtractor
    {
        private readonly string _connectionString;

        public OrderDatabaseExtractor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<object>> ExtractAsync()
        {
            var result = new List<object>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand("SELECT * FROM Orders", conn);
            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new
                {
                    OrderID = reader["OrderID"],
                    OrderDate = reader["OrderDate"]
                });
            }

            return result;
        }
    }
}