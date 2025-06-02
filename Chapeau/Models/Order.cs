namespace Chapeau.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public Employee Employee { get; set; }
        public Table Table { get; set; }
        public DateTime OrderTime { get; set; }//important for bar/kitchen
        public bool IsServed { get; set; }//need to remove
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public string? Notes { get; set; }//missing implementation

        //new attibute from teacher's recomendation
        public bool IsReadyToPay { get; set; } =false;
        public bool IsPaid { get; set; } = false;
        public Order(int orderID, Employee employee, Table table, DateTime orderTime, bool isServed, List<OrderItem> orderItems)
        {
            OrderID = orderID;
            Employee = employee;
            Table = table;
            OrderTime = orderTime;
            IsServed = isServed;
            //IsReadyToPay = isReadyToPay;
            //IsPaid = isPaid;
            OrderItems = orderItems;
        }
        //parameterless ctor
        public Order() { }

    }
}
