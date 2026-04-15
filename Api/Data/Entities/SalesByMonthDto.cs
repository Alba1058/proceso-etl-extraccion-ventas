namespace Api.Data.Entities
{
    public class SalesByMonthDto
    {
        public int Anio { get; init; }
        public int Mes { get; init; }
        public string NombreMes { get; init; } = string.Empty;
        public decimal Total { get; init; }
    }
}
