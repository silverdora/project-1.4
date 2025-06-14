using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class FinishOrderViewModel
    {
        public int OrderID { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TipAmount { get; set; }
        public PaymentType PaymentType { get; set; }
        public string Feedback { get; set; } // Optional
        public decimal LowVatAmount { get; set; }
        public decimal HighVatAmount { get; set; }
    }
}
