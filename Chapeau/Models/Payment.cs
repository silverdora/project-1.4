namespace Chapeau.Models
{
    public class Payment
    {
        public int paymentID { get; set; }
        public int orderID { get; set; }
        public string paymentType { get; set; }//enum 
        public decimal amountPaid { get; set; }
        public decimal tipAmount { get; set; }
        public DateTime paymentDAte { get; set; }

        //my VAT amounts for the order payments
        //should be added to my database but I cannot 

        public decimal lowVATAmount { get; set; }
        public decimal highVATAmount { get; set; }

        // ADDITIONAL PROPERTIES FOR VIEWING ORDER
        public Table Table { get; set; } // Includes table number
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public decimal TotalAmount => OrderItems.Sum(i => i.MenuItem.Price * i.Quantity);





        public Payment()
        {

        }

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
