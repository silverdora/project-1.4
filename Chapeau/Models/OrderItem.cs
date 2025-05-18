namespace Chapeau.Models
{
    public class OrderItem
    {
        public int OrderID { get; set; }
        public MenuItem MenuItem { get; set; }
        public int ItemID { get; set; }
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

        public OrderItem(int itemID, MenuItem menuItem, DateTime includeDate, Status status, int quantity)
        {
            ItemID = itemID;
            MenuItem = menuItem;
            IncludeDate = includeDate;
            Status = status;
            Quantity = quantity;
        }
    }
}
