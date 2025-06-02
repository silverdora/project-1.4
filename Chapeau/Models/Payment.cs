namespace Chapeau.Models
{
    public class Payment
    {
        public int paymentID { get; set; }
        public int orderID { get; set; }
        public string paymentType { get; set; } // Consider using an enum here if appropriate
        public decimal amountPaid { get; set; }
        public decimal tipAmount { get; set; }
        public DateTime paymentDAte { get; set; }

        // VAT amounts that are stored in your database
        public decimal lowVatAmount { get; set; }  // Match with DB field: lowVatAmount
        public decimal highVATAmount { get; set; } // Match with DB field: highVATAmount

        public Payment() { }

        public Payment(int paymentID, int orderID, string paymentType, decimal amountPaid, decimal tipAmount, DateTime paymentDate)
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
