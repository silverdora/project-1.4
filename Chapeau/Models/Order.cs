

namespace Chapeau.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public Employee Employee { get; set; }
        public Table Table { get; set; }
        public DateTime OrderTime { get; set; }
        //public bool IsServed { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        

        public Status Status { get; set; }
        public Order()
        {
            OrderItems = new List<OrderItem>();
        }


        public Order(int orderID, Employee employee, Table table, DateTime orderTime, bool isServed, List<OrderItem> orderItems)
        {
            OrderID = orderID;
            Employee = employee;
            Table = table;
            OrderTime = orderTime;
            //IsServed = isServed;
            OrderItems = orderItems;
        }

        

    }
}
