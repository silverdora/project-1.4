using System.ComponentModel.DataAnnotations;

namespace Chapeau.ViewModels
{
    public class SplitPaymentViewModel
    {
        public int OrderID { get; set; }
        public decimal TotalAmount { get; set; }

        [Range(2, 4, ErrorMessage = "Number of people must be between 2 and 4")]
        public int NumberOfPeople { get; set; } = 2; // Default to 2

        public List<IndividualPayment> Payments { get; set; } = new List<IndividualPayment>();
    }
}