using System.ComponentModel.DataAnnotations;

namespace Chapeau.ViewModels
{
    public class PaymentViewModel
    {
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        public string PaymentMethod { get; set; }
    }
}
