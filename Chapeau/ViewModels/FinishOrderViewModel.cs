using Chapeau.Models;
using System.ComponentModel.DataAnnotations;

namespace Chapeau.ViewModels
{
    public class FinishOrderViewModel
    {
        public int OrderID { get; set; }

        [Required]
        public decimal AmountPaid { get; set; }

        public decimal TipAmount { get; set; }

        [Required]
        public PaymentType PaymentType { get; set; }

        public string? Feedback { get; set; }

        [Required]
        public decimal LowVatAmount { get; set; }

        [Required]
        public decimal HighVatAmount { get; set; }
    }
}

