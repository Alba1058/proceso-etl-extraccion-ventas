using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Application.Result;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using SistemaAnalisisVentas.Domain.Entities.Dwh.Dimensions;
using SistemaAnalisisVentas.Domain.Entities.Dwh.Facts;
using SistemaAnalisisVentas.Persistence.Repositories.Dwh.Context;

namespace Persistence.Repositories.Dwh
{
    public class DwhRepository : IDwhRepository
    {
        private readonly VentasAnaliticaContext _context;
        private const int BulkBatchSize = 5000;

        public DwhRepository(VentasAnaliticaContext context)
        {
            _context = context;
        }

        public async Task LoadCustomersAsync(IEnumerable<DimCliente> customers) { await Task.CompletedTask; }
        public async Task LoadProductsAsync(IEnumerable<DimProducto> products) { await Task.CompletedTask; }
        public async Task LoadTimeAsync(IEnumerable<DimTiempo> times) { await Task.CompletedTask; }
        public async Task LoadBranchesAsync(IEnumerable<DimSucursal> branches) { await Task.CompletedTask; }
        public async Task LoadLocationsAsync(IEnumerable<DimUbicacion> locations) { await Task.CompletedTask; }
        public async Task LoadDataSourcesAsync(IEnumerable<DimFuenteDatos> sources) { await Task.CompletedTask; }
        public async Task LoadSalesAsync(IEnumerable<FactVenta> sales) { await Task.CompletedTask; }

        public async Task<ServiceResult> LoadAnalyticsDataAsync(object preparedDataObj, CancellationToken cancellationToken)
        {
            if (preparedDataObj is not Domain.Models.PreparedSalesData preparedData)
            {
                return ServiceResult.Failure("Los datos no tienen el formato esperado (PreparedSalesData).");
            }

            try
            {
                await _context.Database.EnsureCreatedAsync(cancellationToken);

                //Limpiar Tablas de Hechos y Dimensiones
                var cleanResult = await CleanDimenssionTables(cancellationToken);
                if (!cleanResult.IsSuccess)
                {
                    return cleanResult;
                }

                // Carga Dimensiones
                var loadDimsResult = await LoadDimensionsAsync(preparedData, cancellationToken);
                if (!loadDimsResult.IsSuccess)
                {
                    return loadDimsResult;
                }

                // Carga de los facts
                var loadFactsResult = await LoadFactVentasAsync(preparedData, cancellationToken);
                
                return loadFactsResult;
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error cargando el Data Warehouse: {ex.Message}");
            }
        }

        private async Task<ServiceResult> CleanDimenssionTables(CancellationToken cancellationToken)
        {
            ServiceResult result = null;
            try
            {
                await _context.Fact_Ventas.ExecuteDeleteAsync(cancellationToken);
                await _context.Dim_Producto.ExecuteDeleteAsync(cancellationToken);
                await _context.Dim_Cliente.ExecuteDeleteAsync(cancellationToken);
                await _context.Dim_Tiempo.ExecuteDeleteAsync(cancellationToken);
                await _context.Dim_Sucursal.ExecuteDeleteAsync(cancellationToken);
                await _context.Dim_Ubicacion.ExecuteDeleteAsync(cancellationToken);
                await _context.Dim_Fuente_Datos.ExecuteDeleteAsync(cancellationToken);

                result = ServiceResult.Success("La data de los facts y dimensiones fueron limpiadas.");
            }
            catch (Exception ex)
            {
                result = ServiceResult.Failure($"Error limpiando las tablas: {ex.Message}");
            }

            return result;
        }

        private async Task<ServiceResult> LoadDimensionsAsync(Domain.Models.PreparedSalesData preparedData, CancellationToken cancellationToken)
        {
            try
            {
                var dimClientes = preparedData.Customers.Select(c => new DimCliente
                {
                    ClienteOrigenId = c.CustomerID,
                    NombreCliente = $"{c.FirstName} {c.LastName}".Trim(),
                    Email = c.Email,
                    Telefono = c.Phone,
                    SegmentoCliente = "Estándar",
                    FechaRegistro = DateTime.UtcNow.Date
                }).ToArray();
                await _context.BulkInsertAsync(dimClientes, new BulkConfig { PreserveInsertOrder = true, SetOutputIdentity = true }, cancellationToken: cancellationToken);

                var dimProductos = preparedData.Products.Select(p => new DimProducto
                {
                    ProductoOrigenId = p.ProductID,
                    NombreProducto = p.ProductName,
                    Categoria = p.Category,
                    Marca = "N/A",
                    PrecioBase = p.Price,
                    EstadoProducto = "Activo"
                }).ToArray();
                await _context.BulkInsertAsync(dimProductos, new BulkConfig { PreserveInsertOrder = true, SetOutputIdentity = true }, cancellationToken: cancellationToken);

                var fechasUnicas = preparedData.SalesFacts.Select(f => f.OrderDate.Date).Distinct().ToList();
                var dimTiempos = fechasUnicas.Select(f => new DimTiempo
                {
                    IdTiempo = Convert.ToInt32(f.ToString("yyyyMMdd")),
                    Fecha = f,
                    Dia = f.Day,
                    Mes = f.Month,
                    NombreMes = f.ToString("MMMM", new System.Globalization.CultureInfo("es-ES")),
                    Trimestre = (f.Month - 1) / 3 + 1,
                    Anio = f.Year
                }).ToArray();
                await _context.BulkInsertAsync(dimTiempos, new BulkConfig { PreserveInsertOrder = true }, cancellationToken: cancellationToken);

                var sucursalesUnicas = preparedData.SalesFacts.Select(f => new { f.BranchName, f.BranchAddress }).Distinct().ToList();
                var dimSucursales = sucursalesUnicas.Select(s => new DimSucursal
                {
                    NombreSucursal = s.BranchName,
                    Direccion = s.BranchAddress
                }).ToArray();
                await _context.BulkInsertAsync(dimSucursales, new BulkConfig { PreserveInsertOrder = true, SetOutputIdentity = true }, cancellationToken: cancellationToken);

                var ubicacionesUnicas = preparedData.SalesFacts.Select(f => new { f.Country, f.Region, f.City }).Distinct().ToList();
                var dimUbicaciones = ubicacionesUnicas.Select(u => new DimUbicacion
                {
                    Pais = u.Country,
                    Region = u.Region,
                    Ciudad = u.City
                }).ToArray();
                await _context.BulkInsertAsync(dimUbicaciones, new BulkConfig { PreserveInsertOrder = true, SetOutputIdentity = true }, cancellationToken: cancellationToken);

                var fuentesUnicas = preparedData.SalesFacts.Select(f => new { f.SourceName, f.SourceType }).Distinct().ToList();
                var dimFuentes = fuentesUnicas.Select(s => new DimFuenteDatos
                {
                    NombreFuente = s.SourceName,
                    TipoFuente = s.SourceType,
                    DescripcionFuente = $"Datos extraídos de {s.SourceType}"
                }).ToArray();
                await _context.BulkInsertAsync(dimFuentes, new BulkConfig { PreserveInsertOrder = true, SetOutputIdentity = true }, cancellationToken: cancellationToken);

                return ServiceResult.Success("Dimensiones cargadas exitosamente.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error en LoadDimensionsAsync: {ex.Message}");
            }
        }

        private async Task<ServiceResult> LoadFactVentasAsync(Domain.Models.PreparedSalesData preparedData, CancellationToken cancellationToken)
        {
            try
            {
                var clientesDb = await _context.Dim_Cliente.ToDictionaryAsync(c => c.ClienteOrigenId, c => c.IdCliente, cancellationToken);
                var productosDb = await _context.Dim_Producto.ToDictionaryAsync(p => p.ProductoOrigenId, p => p.IdProducto, cancellationToken);
                var sucursalesDb = await _context.Dim_Sucursal.ToDictionaryAsync(s => s.NombreSucursal + "|" + s.Direccion, s => s.IdSucursal, cancellationToken);
                var ubicacionesDb = await _context.Dim_Ubicacion.ToDictionaryAsync(u => u.Pais + "|" + u.Region + "|" + u.Ciudad, u => u.IdUbicacion, cancellationToken);
                var fuentesDb = await _context.Dim_Fuente_Datos.ToDictionaryAsync(f => f.NombreFuente + "|" + f.TipoFuente, f => f.IdFuente, cancellationToken);

                var factVentas = new List<FactVenta>();
                var processedRecords = 0;
                var skippedRecords = 0;

                foreach (var f in preparedData.SalesFacts)
                {
                    try
                    {
                        if (!clientesDb.TryGetValue(f.CustomerSourceId, out var idCliente)) { skippedRecords++; continue; }
                        if (!productosDb.TryGetValue(f.ProductSourceId, out var idProducto)) { skippedRecords++; continue; }
                        if (!sucursalesDb.TryGetValue(f.BranchName + "|" + f.BranchAddress, out var idSucursal)) { skippedRecords++; continue; }
                        if (!ubicacionesDb.TryGetValue(f.Country + "|" + f.Region + "|" + f.City, out var idUbicacion)) { skippedRecords++; continue; }
                        if (!fuentesDb.TryGetValue(f.SourceName + "|" + f.SourceType, out var idFuente)) { skippedRecords++; continue; }

                        var fact = new FactVenta
                        {
                            NumeroOrdenOrigen = f.OrderNumber,
                            IdProducto = idProducto,
                            IdCliente = idCliente,
                            IdTiempo = Convert.ToInt32(f.OrderDate.ToString("yyyyMMdd")),
                            IdSucursal = idSucursal,
                            IdUbicacion = idUbicacion,
                            IdFuente = idFuente,
                            Cantidad = f.Quantity,
                            PrecioUnitario = f.UnitPrice,
                            TotalVenta = f.TotalSale
                        };

                        factVentas.Add(fact);
                        processedRecords++;

                        if (factVentas.Count >= BulkBatchSize)
                        {
                            await _context.BulkInsertAsync(factVentas, new BulkConfig { PreserveInsertOrder = true }, cancellationToken: cancellationToken);
                            factVentas.Clear();
                        }
                    }
                    catch (Exception)
                    {
                        skippedRecords++;
                    }
                }

                // Insertar registros restantes
                if (factVentas.Any())
                {
                    await _context.BulkInsertAsync(factVentas, new BulkConfig { PreserveInsertOrder = true }, cancellationToken: cancellationToken);
                }

                var message = $"Fact table cargado exitosamente. Registros procesados: {processedRecords}, Registros omitidos: {skippedRecords}";
                return ServiceResult.Success(message);
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error cargando fact table: {ex.Message}");
            }
        }
    }
}
