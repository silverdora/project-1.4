using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class IndividualPayment
    {
        public decimal AmountPaid { get; set; }
        public decimal TipAmount { get; set; }
        public PaymentType PaymentType { get; set; }
        public string Feedback { get; set; }

    }
}
