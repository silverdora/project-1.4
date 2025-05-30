namespace Chapeau.Models
{
    public class OrderItem
    {
        public int OrderID { get; set; }
        public MenuItem MenuItem { get; set; }
        public DateTime OrderDateTime { get; set; }
        public TimeSpan WaitingTime
        {
            get
            {
                return DateTime.Now - OrderDateTime;
            }
        }

        public Status Status { get; set; }
        public int Quantity { get; set; }

        public OrderItem(MenuItem menuItem, DateTime orderDateTime, Status status, int quantity)
        {
            MenuItem = menuItem;
            OrderDateTime = orderDateTime;
            Status = status;
            Quantity = quantity;
        }
    }
}

