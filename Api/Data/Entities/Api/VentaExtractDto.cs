using Api.Data.Entities;
namespace Api.Data.Entities
{
    public class VentaExtractDto
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string SourceName { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
    }
}
