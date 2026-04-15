namespace Domain.Entities.Csv
{
    public class OrderDetailCsv
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        private decimal _unitPrice;
        public decimal UnitPrice 
        { 
            get => _unitPrice > 0 ? _unitPrice : (Quantity > 0 ? TotalPrice / Quantity : 0);
            set => _unitPrice = value; 
        }
        public decimal TotalPrice { get; set; }
    }
}
