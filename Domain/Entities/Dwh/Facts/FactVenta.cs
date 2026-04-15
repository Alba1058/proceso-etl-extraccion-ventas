using System;
using System.ComponentModel.DataAnnotations.Schema;
using SistemaAnalisisVentas.Domain.Entities.Dwh.Dimensions;

namespace SistemaAnalisisVentas.Domain.Entities.Dwh.Facts
{
    public class FactVenta
    {
        public int IdVenta { get; set; }
        public int? NumeroOrdenOrigen { get; set; }
        public int IdProducto { get; set; }
        public int IdCliente { get; set; }
        public int IdTiempo { get; set; }
        public int IdSucursal { get; set; }
        public int IdUbicacion { get; set; }
        public int IdFuente { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal TotalVenta { get; set; }

        // Navigation properties
        public DimProducto Producto { get; set; } = null!;
        public DimCliente Cliente { get; set; } = null!;
        public DimTiempo Tiempo { get; set; } = null!;
        public DimSucursal Sucursal { get; set; } = null!;
        public DimUbicacion Ubicacion { get; set; } = null!;
        public DimFuenteDatos FuenteDatos { get; set; } = null!;
    }
}
