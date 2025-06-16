using System.Text.Json;

namespace Chapeau.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public Employee Employee { get; set; }
        public Table Table { get; set; }
        public DateTime OrderTime { get; set; }
        //public bool IsServed { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
           
        public bool? IsPaid { get; set; } = false;
        public Status Status { get; set; }
        public Order()
        {
            OrderItems = new List<OrderItem>();
        }

        public Order(int orderId, Employee employee, Table table, DateTime orderTime, List<OrderItem> orderItems, bool isPaid)
        {
            OrderId = orderId;
            Employee = employee;
            Table = table;
            OrderTime = orderTime;
            OrderItems = orderItems;
            IsPaid = isPaid;          
        }

        // --- Session handling ---

        private const string SessionKey = "CurrentOrder";

        public void SaveToSession(ISession session)
        {            
            string json = JsonSerializer.Serialize(this);//convert the order object to json
            session.SetString(SessionKey, json);
        }

        public static Order LoadFromSession(ISession session)
        {
            string? json = session.GetString(SessionKey);
            if (string.IsNullOrEmpty(json))
                return new Order();

            return JsonSerializer.Deserialize<Order>(json) ?? new Order();
        }

        public static void ClearFromSession(ISession session)
        {
            session.Remove(SessionKey);
        }       
        // --- Cart  ---

        public void AddOrUpdateItem(MenuItem item, int quantity)
        {
            OrderItem existing = null;

            foreach (OrderItem orderItem in OrderItems)
            {
                if (orderItem.MenuItem.ItemId == item.ItemId)
                {
                    existing = orderItem;
                    break; // Stop the loop once we find the match
                }
            }
            if (existing != null)
            {
                existing.Quantity += quantity;
                existing.IncludeDate = DateTime.Now;
            }
            else
            {
                OrderItems.Add(new OrderItem
                {
                    MenuItem = item,
                    Quantity = quantity,
                    IncludeDate = DateTime.Now,
                    Status = Status.Ordered
                });
            }
        }
        public void IncreaseOrDecreaseQuantity(int itemId, int adjustment)
        {
            OrderItem item = null;

            foreach (OrderItem orderItem in OrderItems)
            {
                if (orderItem.MenuItem.ItemId == itemId)
                {
                    item = orderItem;
                    break;
                }
            }
            if (item != null)
            {
                item.Quantity += adjustment;
                if (item.Quantity <= 0)
                {
                    OrderItems.Remove(item);
                }
            }
        }
    }
}
