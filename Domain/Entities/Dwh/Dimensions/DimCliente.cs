using System;

namespace SistemaAnalisisVentas.Domain.Entities.Dwh.Dimensions
{
    public class DimCliente
    {
        public int IdCliente { get; set; }
        public int ClienteOrigenId { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string SegmentoCliente { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
    }
}
