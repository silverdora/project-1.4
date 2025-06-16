using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;
using Chapeau.Repository.Interface;

namespace Chapeau.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;
        private readonly ITableRepository _tableRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderRepository(IConfiguration configuration, ITableRepository tableRepository, IEmployeeRepository employeeRepository, IOrderItemRepository orderItemRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _tableRepository = tableRepository;
            _employeeRepository = employeeRepository;
            _orderItemRepository = orderItemRepository;
        }

        //take an order
        public void InsertOrder(Order order)
        {
            string query = "INSERT INTO [Order] (employeeID, tableID, orderTime, isPaid) " +
                           "VALUES (@employeeID, @tableID, @orderTime, 0); " +
                           "SELECT SCOPE_IDENTITY();";
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@employeeID", order.Employee.employeeID);
                    command.Parameters.AddWithValue("@tableID", order.Table.TableId);
                    command.Parameters.AddWithValue("@orderTime", order.OrderTime);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    order.OrderId = Convert.ToInt32(result);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while inserting order.", ex);
            }

        }

        //kitchen or bar
        public List<Order> GetOrdersByStatus(Status status, string type, DateTime createdAfter)
        {
            List<Order> orders = new List<Order>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * " +
                    "FROM [Order] " +
                    "WHERE orderID IN (" +
                    "SELECT orderID FROM OrderItem JOIN MenuItem ON OrderItem.itemID = MenuItem.itemID " +
                    "WHERE item_type = @type AND [status] = @status) AND orderTime > @time " +
                    "ORDER BY orderTime;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", status.ToString());
                command.Parameters.AddWithValue("@type", type.ToString());
                command.Parameters.AddWithValue("@time", createdAfter);
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Order order = ReadOrder(reader, status, type);
                    orders.Add(order);
                }
                reader.Close();
            }
            return orders;
        }

        private Order ReadOrder(SqlDataReader reader, Status status, string type)
        {
            int orderId = (int)reader["orderID"];

            int employeeID = (int)reader["employeeID"];
            Employee employee = _employeeRepository.GetEmployeeByID(employeeID);

            int tableID = (int)reader["tableID"];
            Table table = _tableRepository.GetTableByID(tableID);

            DateTime orderTime = (DateTime)reader["orderTime"];
            bool isPaid = Convert.ToBoolean(reader["isPaid"]);
            //bool isServed = true;
            List<OrderItem> orderItems = _orderItemRepository.GetOrderItemsByOrderID(orderId, status, type);

            return new Order(orderId, employee, table, orderTime, orderItems, isPaid);
        }

        //get active order by table to double check that if there is any existent order, avoiding to creating everytime a new
        public Order? GetActiveOrderByTableId(int tableId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"
                               SELECT * FROM [Order]
                               WHERE TableId = @tableId AND IsPaid = 0";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@tableId", tableId);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        return MapToOrder(reader);
                    }
                    return null;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while retrieving active order.", ex);
            }
        }
        private Order MapToOrder(SqlDataReader reader)
        {
            int tableId = Convert.ToInt32(reader["tableID"]);
            Table matchedTable = _tableRepository.GetTableById(tableId);

            return new Order
            {
                OrderId = Convert.ToInt32(reader["orderID"]),
                OrderTime = Convert.ToDateTime(reader["orderTime"]),
                IsPaid = Convert.ToBoolean(reader["isPaid"]),
                Table = matchedTable,
                Employee = _employeeRepository.GetEmployeeByID(Convert.ToInt32(reader["employeeID"])),
            };
        }
        //get active order by table to double check that if there is any existent order, avoiding to creating everytime a new
       
        

        public Order GetOrderById(int orderId)
        {
            try
            {
                string query = @"
                    SELECT o.orderID, o.tableID, o.orderTime, o.isPaid,
                           oi.itemID, oi.quantity, oi.includeDate, oi.status AS itemStatus,
                           m.itemID, m.item_name, m.price, m.VATPercent
                    FROM [Order] o
                    JOIN OrderItem oi ON o.orderID = oi.orderID
                    JOIN MenuItem m ON oi.itemID = m.itemID
                    WHERE o.orderID = @orderId";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        return MapOrderFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve order {orderId}", ex);
            }
        }

        public void MarkOrderAsPaid(int orderId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "UPDATE [Order] SET isPaid = 1 WHERE orderID = @orderId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@orderId", orderId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to mark order {orderId} as paid", ex);
            }
        }

        private Order MapOrderFromReader(SqlDataReader reader)
        {
            Order order = null;
            int itemCount = 0;
            while (reader.Read())
            {
                if (order == null)
                {
                    order = new Order
                    {
                        OrderId = reader.GetInt32(0),
                        Table = new Table { TableId = reader.GetInt32(1) },
                        OrderTime = reader.GetDateTime(2),
                        Status = reader.GetBoolean(3) ? Status.Served : Status.Ordered,
                        OrderItems = new List<OrderItem>()
                    };
                }

                MenuItem menuItem = new MenuItem
                {
                    ItemId = reader.GetInt32(8),
                    Item_name = reader.GetString(9),
                    Price = reader.GetDecimal(10),
                    VATPercent = reader.GetDecimal(11)
                };

                OrderItem item = new OrderItem
                {
                    OrderItemId = reader.GetInt32(4),
                    Quantity = reader.GetInt32(5),
                    IncludeDate = reader.GetDateTime(6),
                    Status = (Status)Enum.Parse(typeof(Status), reader.GetString(7), true),
                    MenuItem = menuItem
                };

                order.OrderItems.Add(item);
                itemCount++;
            }
            if (order == null)
                throw new Exception("MapOrderFromReader: No rows returned from SQL query.");
            if (itemCount == 0)
                throw new Exception("MapOrderFromReader: Order found but no items were mapped.");
            return order;
        }
    }
}


    


