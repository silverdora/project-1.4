namespace Chapeau.ViewModels
{
    public class OrderItemViewModel
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VATRate { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;

    }
}
