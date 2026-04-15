using Api.Data.Entities;
using Api.Data.Interfaces;
using Api.Data.Persistence;
using Dapper;

namespace Api.Data.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly DapperContext _context;

        public ReportRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReportRowDto>> GetVentasPorProductoAsync(CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT p.nombre_producto AS Nombre, SUM(f.total_venta) AS Total
                FROM dbo.Fact_Ventas f
                INNER JOIN dbo.Dim_Producto p ON f.id_producto = p.id_producto
                GROUP BY p.nombre_producto
                ORDER BY Total DESC;
                """;

            using var connection = _context.CreateDataWarehouseConnection();
            return await connection.QueryAsync<ReportRowDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<ReportRowDto>> GetVentasPorClienteAsync(CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT c.nombre_cliente AS Nombre, SUM(f.total_venta) AS Total
                FROM dbo.Fact_Ventas f
                INNER JOIN dbo.Dim_Cliente c ON f.id_cliente = c.id_cliente
                GROUP BY c.nombre_cliente
                ORDER BY Total DESC;
                """;

            using var connection = _context.CreateDataWarehouseConnection();
            return await connection.QueryAsync<ReportRowDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<SalesByMonthDto>> GetVentasPorMesAsync(CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT t.anio AS Anio, t.mes AS Mes, t.nombre_mes AS NombreMes, SUM(f.total_venta) AS Total
                FROM dbo.Fact_Ventas f
                INNER JOIN dbo.Dim_Tiempo t ON f.id_tiempo = t.id_tiempo
                GROUP BY t.anio, t.mes, t.nombre_mes
                ORDER BY t.anio, t.mes;
                """;

            using var connection = _context.CreateDataWarehouseConnection();
            return await connection.QueryAsync<SalesByMonthDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<TopProductDto>> GetTopProductosAsync(CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT TOP 5 p.nombre_producto AS Nombre, SUM(f.cantidad) AS TotalUnidades
                FROM dbo.Fact_Ventas f
                INNER JOIN dbo.Dim_Producto p ON f.id_producto = p.id_producto
                GROUP BY p.nombre_producto
                ORDER BY TotalUnidades DESC;
                """;

            using var connection = _context.CreateDataWarehouseConnection();
            return await connection.QueryAsync<TopProductDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<TopClientDto>> GetTopClientesAsync(CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT TOP 5 c.nombre_cliente AS Nombre, COUNT(DISTINCT f.numero_orden_origen) AS TotalOrdenes
                FROM dbo.Fact_Ventas f
                INNER JOIN dbo.Dim_Cliente c ON f.id_cliente = c.id_cliente
                GROUP BY c.nombre_cliente
                ORDER BY TotalOrdenes DESC;
                """;

            using var connection = _context.CreateDataWarehouseConnection();
            return await connection.QueryAsync<TopClientDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        }
    }
}
