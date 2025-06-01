namespace Chapeau.ViewModels
{
    public class OrderViewModel
    {
        public int OrderID { get; set; }
        public int TableNumber { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal LowVATAmount { get; set; }
        public decimal HighVATAmount { get; set; }

        public string PaymentType { get; set; }
        public decimal TipAmount { get; set; }
    }

    public class OrderItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
    

