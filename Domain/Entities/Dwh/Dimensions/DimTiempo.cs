using System;

namespace SistemaAnalisisVentas.Domain.Entities.Dwh.Dimensions
{
    public class DimTiempo
    {
        public int IdTiempo { get; set; } // PK: Formato YYYYMMDD
        public DateTime Fecha { get; set; }
        public int Dia { get; set; }
        public int Mes { get; set; }
        public string NombreMes { get; set; } = string.Empty;
        public int Trimestre { get; set; }
        public int Anio { get; set; }
    }
}
