using System;

namespace SistemaAnalisisVentas.Domain.Entities.Dwh.Dimensions
{
    public class DimSucursal
    {
        public int IdSucursal { get; set; }
        public string NombreSucursal { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
    }
}
