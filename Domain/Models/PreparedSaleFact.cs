namespace Domain.Models
{
    public class PreparedSaleFact
    {
        public int OrderNumber { get; init; }
        public int ProductSourceId { get; init; }
        public int CustomerSourceId { get; init; }
        public DateTime OrderDate { get; init; }
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal TotalSale { get; init; }
        public string SourceName { get; init; } = string.Empty;
        public string SourceType { get; init; } = string.Empty;
        public string BranchName { get; init; } = "Sucursal Principal";
        public string BranchAddress { get; init; } = "No especificada";
        public string Country { get; init; } = "N/A";
        public string Region { get; init; } = "Region no especificada";
        public string City { get; init; } = "N/A";
    }
}
