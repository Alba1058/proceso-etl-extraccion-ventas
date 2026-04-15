using System;

namespace SistemaAnalisisVentas.Domain.Entities.Dwh.Dimensions
{
    public class DimProducto
    {
        public int IdProducto { get; set; }
        public int ProductoOrigenId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public decimal PrecioBase { get; set; }
        public string EstadoProducto { get; set; } = string.Empty;
    }
}
