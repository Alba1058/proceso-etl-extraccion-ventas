using Microsoft.Data.SqlClient;

namespace Api.Data.Persistence
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection CreateDataWarehouseConnection()
        {
            var connectionString = _configuration.GetConnectionString("DWH")
                ?? _configuration.GetConnectionString("DataWarehouseDb")
                ?? throw new InvalidOperationException("No se configuro la conexion del Data Warehouse.");

            return new SqlConnection(connectionString);
        }
    }
}
