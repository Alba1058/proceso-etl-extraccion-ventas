using System;

namespace SistemaAnalisisVentas.Domain.Entities.Dwh.Dimensions
{
    public class DimFuenteDatos
    {
        public int IdFuente { get; set; }
        public string NombreFuente { get; set; } = string.Empty;
        public string TipoFuente { get; set; } = string.Empty;
        public string DescripcionFuente { get; set; } = string.Empty;
    }
}
