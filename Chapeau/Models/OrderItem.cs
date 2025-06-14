namespace Chapeau.Models
{
    public class OrderItem
    {
        public MenuItem MenuItem { get; set; }
        public int OrderItemId { get; set; }//now nullable to keep OrderService alive
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
        

        public OrderItem(MenuItem menuItem, int orderItemId, DateTime includeDate, Status status, int quantity, string? comment)
        {
            MenuItem = menuItem;
            OrderItemId = orderItemId;
            IncludeDate = includeDate;
            Status = status;
            Quantity = quantity;
            Comment = comment;
        }
    }
}

