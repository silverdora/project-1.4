

namespace Chapeau.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public Employee Employee { get; set; }
        public Table Table { get; set; }
        public DateTime OrderTime { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        

        public bool? IsPaid { get; set; } = false;
        public Status Status { get; set; }
        public Order()
        {
            OrderItems = new List<OrderItem>();
        }

        public Order(int orderID, Employee employee, Table table, DateTime orderTime, List<OrderItem> orderItems, bool isPaid)
        {
            OrderID = orderID;
            Employee = employee;
            Table = table;
            OrderTime = orderTime;
            OrderItems = orderItems;
            IsPaid = isPaid;
        }      
    }
}
