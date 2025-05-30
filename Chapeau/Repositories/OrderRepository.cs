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

        public OrderRepository(IConfiguration configuration, ITableRepository tableRepository, IEmployeeRepository employeeRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _tableRepository = tableRepository;
            _employeeRepository = employeeRepository;
        }
        //Insert + return full Order
        public Order TakeNewOrder(int tableId, Employee employee)
        {
            int newOrderID = InsertNewOrder(tableId, employee);
            return GetOrderByID(newOrderID);
        }

        //Get Order by ID
        public Order GetOrderByID(int orderID)
        {
            string query = "SELECT * FROM [Order] WHERE orderID = @orderID";
            SqlParameter[] parameters = {new SqlParameter("@orderID", orderID)  };

            
            Order order = MapOrdersFromQuery(query, parameters).FirstOrDefault();
                       
            if (order != null)
            {
                order.OrderItems = GetOrderItemsForOrder(orderID); 
            }

            return order;
        }
       
        public bool OrderItemExists(int orderID, int itemID)
        {
            bool exists = false;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM OrderItem WHERE orderID = @orderID AND itemID = @itemID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@orderID", orderID);
                    command.Parameters.AddWithValue("@itemID", itemID);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // If we can read one row, it exists
                        {
                            exists = true;
                        }
                    }
                }
            }
            return exists;
        }
        public void InsertOrderItem(int orderID, OrderItem item)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = "INSERT INTO OrderItem (orderID, itemID, includeDate, status, quantity) VALUES (@orderID, @itemID, @includeDate, @status, @quantity)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@orderID", orderID);
            command.Parameters.AddWithValue("@itemID", item.ItemID);
            command.Parameters.AddWithValue("@includeDate", item.IncludeDate);
            command.Parameters.AddWithValue("@status", item.Status.ToString());
            command.Parameters.AddWithValue("@quantity", item.Quantity);

            command.ExecuteNonQuery();
        }

        public void UpdateOrderItem(int orderID, OrderItem item)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = "UPDATE OrderItem SET quantity = @quantity WHERE orderID = @orderID AND itemID = @itemID";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@orderID", orderID);
            command.Parameters.AddWithValue("@itemID", item.ItemID);
            command.Parameters.AddWithValue("@quantity", item.Quantity);

            command.ExecuteNonQuery();
        }
        private List<OrderItem> GetOrderItemsForOrder(int orderID)
        {
            var items = new List<OrderItem>();

            string query = @"
                           SELECT oi.*, mi.item_name, mi.description, mi.price, mi.VATpercent, mi.category, mi.stockQuantity
                           FROM OrderItem oi
                           JOIN MenuItem mi ON oi.itemID = mi.itemID
                           WHERE oi.orderID = @orderID";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@orderID", orderID);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var menuItem = new MenuItem
                        {
                            ItemID = (int)reader["itemID"],
                            Item_name = reader["item_name"].ToString(),
                            Description = reader["description"].ToString(),
                            Price = (decimal)reader["price"],
                            VATPercent = (decimal)reader["VATpercent"],
                            Category = reader["category"].ToString(),
                            StockQuantity = (int)reader["stockQuantity"]
                        };

                        var orderItem = new OrderItem
                        (
                            itemID: menuItem.ItemID,
                            menuItem: menuItem,
                            includeDate: (DateTime)reader["includeDate"],
                            status: Enum.Parse<Status>(reader["status"].ToString()),
                            quantity: (int)reader["quantity"]
                        );

                        items.Add(orderItem);
                    }
                }
            }

            return items;
        }

        private List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            List<OrderItem> items = new List<OrderItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                                SELECT oi.*, mi.*
                                FROM OrderItem oi
                                JOIN MenuItem mi ON oi.itemID = mi.itemID
                                WHERE oi.orderID = @orderID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@orderID", orderId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MenuItem menuItem = new MenuItem
                            {
                                ItemID = (int)reader["itemID"],
                                Item_name = reader["item_name"].ToString(),
                                Description = reader["description"].ToString(),
                                Price = (decimal)reader["price"],
                                VATPercent = (decimal)reader["VATpercent"],
                                Category = reader["category"].ToString(),
                                StockQuantity = (int)reader["stockQuantity"]
                            };

                            OrderItem orderItem = new OrderItem(
                                itemID: (int)reader["itemID"],
                                menuItem: menuItem,
                                includeDate: (DateTime)reader["includeDate"],
                                status: Enum.Parse<Status>(reader["status"].ToString()),
                                quantity: (int)reader["quantity"]
                            );

                            items.Add(orderItem);
                        }
                    }
                }
            }

            return items;
        }


        //Private helper to insert only
        private int InsertNewOrder(int tableId, Employee employee)
        {
            int newOrderID;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                               INSERT INTO [Order] (employeeID, tableID, orderTime)
                               VALUES (@employeeID, @tableID, @orderTime);
                               SELECT CAST(SCOPE_IDENTITY() AS int);";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@employeeID", employee.employeeID);
                    command.Parameters.AddWithValue("@tableID", tableId);
                    command.Parameters.AddWithValue("@orderTime", DateTime.Now);

                    object result = command.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                    {
                        throw new Exception("Failed to retrieve new Order ID.");
                    }

                    newOrderID = Convert.ToInt32(result);
                }
            }

            return newOrderID;
        }

        // Private helper to map SQL rows to Order objects
        private List<Order> MapOrdersFromQuery(string query, SqlParameter[] parameters)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddRange(parameters);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int orderId = Convert.ToInt32(reader["orderID"]);
                        int employeeId = Convert.ToInt32(reader["employeeID"]);
                        int tableId = Convert.ToInt32(reader["tableID"]);
                        DateTime orderTime = Convert.ToDateTime(reader["ordertime"]);
                        Employee employee = _employeeRepository.GetEmployeeByID(employeeId);
                        Table table = GetTableById(tableId);

                        orders.Add(new Order(
                            orderID: orderId,
                            employee: employee,
                            table: table,
                            orderTime: orderTime,
                           orderItems: GetOrderItemsByOrderId(orderId)
                        ));
                    }
                }
            }

            return orders;
        }

        //Private helper to get full Table object 
        private Table GetTableById(int tableId)
        {
            List<Table> tables = _tableRepository.GetAllTables();
            foreach (Table t in tables)
            {
                if (t.TableId == tableId)
                    return t;
            }

            throw new Exception($"Table with ID {tableId} not found.");
        }
    }
}

