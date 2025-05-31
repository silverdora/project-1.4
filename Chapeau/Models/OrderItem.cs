namespace Chapeau.Models
{
    public class OrderItem
    {
        public int OrderID { get; set; }
        public MenuItem MenuItem { get; set; }
        public DateTime IncludeDate { get; set; }//change to OrderDateTime
        public TimeSpan WaitingTime
        {
            get
            {
                return DateTime.Now - IncludeDate;
            }
        }

        public Status Status { get; set; }
        public int Quantity { get; set; }

        // parameterless constructor
        public OrderItem() { }
        public OrderItem(MenuItem menuItem, DateTime includeDate, Status status, int quantity)
        {
            MenuItem = menuItem;
            IncludeDate = includeDate;
            Status = status;
            Quantity = quantity;
        }
     
    }
}

