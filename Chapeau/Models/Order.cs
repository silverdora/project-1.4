namespace Chapeau.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        // public Employee Employee { get; set; }       must create an object
        // public Table Table { get; set; }             must create an object
        public DateTime OrderTime { get; set; }
        public bool IsServed { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
