namespace Chapeau.Models
{
    public class Payment
    {
        public int paymentID { get; set; }
        public int orderID { get; set; }
        public PaymentType paymentType { get; set; }
        public decimal amountPaid { get; set; }
        public decimal tipAmount { get; set; }
        public DateTime paymentDAte { get; set; }

        public string Feedback { get; set; }

        // VAT amounts that are stored in your database
        public decimal lowVatAmount { get; set; }  // Match with DB field: lowVatAmount
        public decimal highVATAmount { get; set; } // Match with DB field: highVATAmount

        public Payment() { }

        public Payment(int paymentID, int orderID, PaymentType paymentType, decimal amountPaid, decimal tipAmount, DateTime paymentDate)
        {
            this.paymentID = paymentID;
            this.orderID = orderID;
            this.paymentType = paymentType;
            this.amountPaid = amountPaid;
            this.tipAmount = tipAmount;
            this.paymentDAte = paymentDate;
        }
    }
}
