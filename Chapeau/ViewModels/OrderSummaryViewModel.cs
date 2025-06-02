namespace Chapeau.ViewModels
{
    public class OrderSummaryViewModel
    {
        public int OrderID { get; set; }
        public int TableNumber { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal LowVAT { get; set; }
        public decimal HighVAT { get; set; }
    }
}



