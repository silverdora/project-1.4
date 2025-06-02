namespace Chapeau.ViewModels
{
    public class SplitPaymentViewModel
    {
        public int OrderID { get; set; }
        public decimal TotalAmount { get; set; }
        public int NumberOfPeople { get; set; }
        public List<IndividualPayment> Payments { get; set; } = new List<IndividualPayment>();
    }
}
