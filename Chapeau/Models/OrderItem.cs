namespace Chapeau.Models
{
    public class OrderItem
    {
        public MenuItem MenuItem { get; set; }
        public DateTime IncludeDate { get; set; }
        public string Status { get; set; }
        public int Quantity { get; set; }

        public OrderItem(MenuItem menuItem, DateTime includeDate, string status, int quantity)
        {
            MenuItem = menuItem;
            IncludeDate = includeDate;
            Status = status;
            Quantity = quantity;
        }
    }
}
