namespace Chapeau.Models
{
    public class OrderItem
    {
        public int? OrderItemID { get; set; } //now nullable to keep OrderService alive
        public MenuItem MenuItem { get; set; }
        public DateTime IncludeDate { get; set; }
        public TimeSpan WaitingTime
        {
            get
            {
                return DateTime.Now - IncludeDate;
            }
        }
        public Status Status { get; set; }
        public int Quantity { get; set; }
        public string? Comment { get; set; }
        public OrderItem()
        {

        }

        public OrderItem(int? orderItemID, MenuItem menuItem, DateTime includeDate, Status status, int quantity, string comment)
        {
            OrderItemID = orderItemID;
            MenuItem = menuItem;
            IncludeDate = includeDate;
            Status = status;
            Quantity = quantity;
            Comment = comment;
        }
    }
}

