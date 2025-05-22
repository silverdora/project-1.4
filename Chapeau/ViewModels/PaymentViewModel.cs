namespace Chapeau.ViewModels
{
    public class PaymentViewModel
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // e.g. "Cash", "Card"
    }
}
