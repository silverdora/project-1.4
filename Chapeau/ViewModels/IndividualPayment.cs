using Chapeau.Models;
using System.ComponentModel.DataAnnotations;

namespace Chapeau.ViewModels
{
    public class IndividualPayment
    {
        [Required(ErrorMessage = "Amount paid is required")]
        public decimal AmountPaid { get; set; }
        public decimal TipAmount { get; set; }
        [Required(ErrorMessage = "Payment type is required")]
        public PaymentType PaymentType { get; set; }
        public string? Feedback { get; set; }
    }
}

