using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Repositories;
using Microsoft.EntityFrameworkCore;
using SistemaAnalisisVentas.Domain.Entities.Dwh.Dimensions;
using SistemaAnalisisVentas.Domain.Entities.Dwh.Facts;
using SistemaAnalisisVentas.Persistence.Repositories.Dwh.Context;

namespace Persistence.Repositories.Dwh
{
    public class DwhRepository : IDwhRepository
    {
        private readonly VentasAnaliticaContext _context;

        public DwhRepository(VentasAnaliticaContext context)
        {
            _context = context;
        }

        public async Task LoadCustomersAsync(IEnumerable<DimCliente> customers)
        {
            // Simple Upsert logic or AddRange if using distinct logic beforehand
            // EF Core Bulk extensions or basic foreach usually applied here.
            foreach (var c in customers)
            {
                var exists = await _context.Dim_Cliente.AnyAsync(x => x.ClienteOrigenId == c.ClienteOrigenId);
                if (!exists)
                {
                    _context.Dim_Cliente.Add(c);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task LoadProductsAsync(IEnumerable<DimProducto> products)
        {
            foreach (var p in products)
            {
                var exists = await _context.Dim_Producto.AnyAsync(x => x.ProductoOrigenId == p.ProductoOrigenId);
                if (!exists)
                {
                    _context.Dim_Producto.Add(p);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task LoadTimeAsync(IEnumerable<DimTiempo> times)
        {
            foreach (var t in times)
            {
                var exists = await _context.Dim_Tiempo.AnyAsync(x => x.IdTiempo == t.IdTiempo);
                if (!exists)
                {
                    _context.Dim_Tiempo.Add(t);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task LoadBranchesAsync(IEnumerable<DimSucursal> branches)
        {
            foreach (var b in branches)
            {
                var exists = await _context.Dim_Sucursal.AnyAsync(x => x.NombreSucursal == b.NombreSucursal && x.Direccion == b.Direccion);
                if (!exists)
                {
                    _context.Dim_Sucursal.Add(b);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task LoadLocationsAsync(IEnumerable<DimUbicacion> locations)
        {
            foreach (var loc in locations)
            {
                var exists = await _context.Dim_Ubicacion.AnyAsync(x => x.Pais == loc.Pais && x.Region == loc.Region && x.Ciudad == loc.Ciudad);
                if (!exists)
                {
                    _context.Dim_Ubicacion.Add(loc);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task LoadDataSourcesAsync(IEnumerable<DimFuenteDatos> sources)
        {
            foreach (var s in sources)
            {
                var exists = await _context.Dim_Fuente_Datos.AnyAsync(x => x.NombreFuente == s.NombreFuente && x.TipoFuente == s.TipoFuente);
                if (!exists)
                {
                    _context.Dim_Fuente_Datos.Add(s);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task LoadSalesAsync(IEnumerable<FactVenta> sales)
        {
            await _context.Fact_Ventas.AddRangeAsync(sales);
            await _context.SaveChangesAsync();
        }

        public async Task<Application.Result.ServiceResult> LoadAnalyticsDataAsync(object preparedDataObj, System.Threading.CancellationToken cancellationToken)
        {
            if (preparedDataObj is not Domain.Models.PreparedSalesData preparedData)
            {
                return Application.Result.ServiceResult.Failure("Los datos no tienen el formato esperado (PreparedSalesData).");
            }

            try
            {
                await _context.Database.EnsureCreatedAsync(cancellationToken);

                await _context.Fact_Ventas.ExecuteDeleteAsync(cancellationToken);
                await _context.Dim_Producto.ExecuteDeleteAsync(cancellationToken);
                await _context.Dim_Cliente.ExecuteDeleteAsync(cancellationToken);
                await _context.Dim_Tiempo.ExecuteDeleteAsync(cancellationToken);
                await _context.Dim_Sucursal.ExecuteDeleteAsync(cancellationToken);
                await _context.Dim_Ubicacion.ExecuteDeleteAsync(cancellationToken);
                await _context.Dim_Fuente_Datos.ExecuteDeleteAsync(cancellationToken);

                var dimClientes = System.Linq.Enumerable.Select(preparedData.Customers, c => new DimCliente
                {
                    ClienteOrigenId = c.CustomerID,
                    NombreCliente = $"{c.FirstName} {c.LastName}".Trim(),
                    Email = c.Email,
                    Telefono = c.Phone,
                    SegmentoCliente = "Estándar",
                    FechaRegistro = System.DateTime.UtcNow.Date
                }).ToList();
                await LoadCustomersAsync(dimClientes);

                var dimProductos = System.Linq.Enumerable.Select(preparedData.Products, p => new DimProducto
                {
                    ProductoOrigenId = p.ProductID,
                    NombreProducto = p.ProductName,
                    Categoria = p.Category,
                    Marca = "N/A",
                    PrecioBase = p.Price,
                    EstadoProducto = "Activo"
                }).ToList();
                await LoadProductsAsync(dimProductos);

                var fechasUnicas = System.Linq.Enumerable.Distinct(System.Linq.Enumerable.Select(preparedData.SalesFacts, f => f.OrderDate.Date)).ToList();
                var dimTiempos = System.Linq.Enumerable.Select(fechasUnicas, f => new DimTiempo
                {
                    IdTiempo = System.Convert.ToInt32(f.ToString("yyyyMMdd")),
                    Fecha = f,
                    Dia = f.Day,
                    Mes = f.Month,
                    NombreMes = f.ToString("MMMM", new System.Globalization.CultureInfo("es-ES")),
                    Trimestre = (f.Month - 1) / 3 + 1,
                    Anio = f.Year
                }).ToList();
                await LoadTimeAsync(dimTiempos);

                var sucursalesUnicas = System.Linq.Enumerable.Distinct(System.Linq.Enumerable.Select(preparedData.SalesFacts, f => new { f.BranchName, f.BranchAddress })).ToList();
                var dimSucursales = System.Linq.Enumerable.Select(sucursalesUnicas, s => new DimSucursal
                {
                    NombreSucursal = s.BranchName,
                    Direccion = s.BranchAddress
                }).ToList();
                await LoadBranchesAsync(dimSucursales);

                var ubicacionesUnicas = System.Linq.Enumerable.Distinct(System.Linq.Enumerable.Select(preparedData.SalesFacts, f => new { f.Country, f.Region, f.City })).ToList();
                var dimUbicaciones = System.Linq.Enumerable.Select(ubicacionesUnicas, u => new DimUbicacion
                {
                    Pais = u.Country,
                    Region = u.Region,
                    Ciudad = u.City
                }).ToList();
                await LoadLocationsAsync(dimUbicaciones);

                var fuentesUnicas = System.Linq.Enumerable.Distinct(System.Linq.Enumerable.Select(preparedData.SalesFacts, f => new { f.SourceName, f.SourceType })).ToList();
                var dimFuentes = System.Linq.Enumerable.Select(fuentesUnicas, s => new DimFuenteDatos
                {
                    NombreFuente = s.SourceName,
                    TipoFuente = s.SourceType,
                    DescripcionFuente = $"Datos extraídos de {s.SourceType}"
                }).ToList();
                await LoadDataSourcesAsync(dimFuentes);

                var clientesDb = await _context.Dim_Cliente.ToDictionaryAsync(c => c.ClienteOrigenId, c => c.IdCliente, cancellationToken);
                var productosDb = await _context.Dim_Producto.ToDictionaryAsync(p => p.ProductoOrigenId, p => p.IdProducto, cancellationToken);
                var sucursalesDb = await _context.Dim_Sucursal.ToDictionaryAsync(s => s.NombreSucursal + "|" + s.Direccion, s => s.IdSucursal, cancellationToken);
                var ubicacionesDb = await _context.Dim_Ubicacion.ToDictionaryAsync(u => u.Pais + "|" + u.Region + "|" + u.Ciudad, u => u.IdUbicacion, cancellationToken);
                var fuentesDb = await _context.Dim_Fuente_Datos.ToDictionaryAsync(f => f.NombreFuente + "|" + f.TipoFuente, f => f.IdFuente, cancellationToken);

                var factVentas = new List<FactVenta>();
                foreach (var f in preparedData.SalesFacts)
                {
                    if (!clientesDb.TryGetValue(f.CustomerSourceId, out var idCliente)) continue;
                    if (!productosDb.TryGetValue(f.ProductSourceId, out var idProducto)) continue;
                    if (!sucursalesDb.TryGetValue(f.BranchName + "|" + f.BranchAddress, out var idSucursal)) continue;
                    if (!ubicacionesDb.TryGetValue(f.Country + "|" + f.Region + "|" + f.City, out var idUbicacion)) continue;
                    if (!fuentesDb.TryGetValue(f.SourceName + "|" + f.SourceType, out var idFuente)) continue;

                    factVentas.Add(new FactVenta
                    {
                        NumeroOrdenOrigen = f.OrderNumber,
                        IdProducto = idProducto,
                        IdCliente = idCliente,
                        IdTiempo = System.Convert.ToInt32(f.OrderDate.ToString("yyyyMMdd")),
                        IdSucursal = idSucursal,
                        IdUbicacion = idUbicacion,
                        IdFuente = idFuente,
                        Cantidad = f.Quantity,
                        PrecioUnitario = f.UnitPrice,
                        TotalVenta = f.TotalSale
                    });
                }

                await LoadSalesAsync(factVentas);

                return Application.Result.ServiceResult.Success($"Carga de datos completada exitosamente. Hechos insertados: {factVentas.Count}");
            }
            catch (System.Exception ex)
            {
                return Application.Result.ServiceResult.Failure($"Error cargando el Data Warehouse: {ex.Message}");
            }
        }
    }
}
