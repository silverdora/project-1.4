using System;
using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;
//using System.Data.SqlClient;

using Humanizer;

namespace Chapeau.Repositories
{
	public class RunningOrdersRepository : IRunningOrdersRepository
	{
        private readonly string _connectionString;

        public RunningOrdersRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        

        public void ChangeOrderStatus(int itemID, Status status)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"UPDATE OrderItem " +
                    $"SET status = @status " +
                    $"WHERE orderID = @Id";

                SqlCommand command = new SqlCommand(query, connection);

                // Preventing SQL injections
                command.Parameters.AddWithValue("@Id", itemID);
                command.Parameters.AddWithValue("@status", status.ToString());

                command.Connection.Open();
                int nrOfRowsAffected = command.ExecuteNonQuery();
                if (nrOfRowsAffected != 1)
                {
                    throw new Exception("Changing status failed!");
                }
            }
        }

        

        public List<Order> GetAllBarOrders()
        {
            List<Order> orders = new List<Order>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * " +
                    "FROM [Order] " +
                    "WHERE orderID IN ( " +
                    "SELECT orderID FROM OrderItem JOIN MenuItem ON OrderItem.itemID = MenuItem.itemID " +
                    "WHERE item_type = 'Drink' AND [status] != 'Completed') " +
                    "ORDER BY orderTime; ";
                SqlCommand command = new SqlCommand(query, connection);
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Order order = ReadOrder(reader);
                    orders.Add(order);
                }
                reader.Close();
            }
            return orders;
        }
        public List<Order> GetAllKitchenOrders()
        {
            List<Order> orders = new List<Order>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * " +
                    "FROM [Order] " +
                    "WHERE orderID IN ( " +
                    "SELECT orderID FROM OrderItem JOIN MenuItem ON OrderItem.itemID = MenuItem.itemID " +
                    "WHERE item_type = 'Dish' AND [status] != 'Completed') " +
                    "ORDER BY orderTime; ";
                SqlCommand command = new SqlCommand(query, connection);
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Order order = ReadOrder(reader);
                    orders.Add(order);
                }
                reader.Close();
            }
            return orders;
        }

        public List<Order> GetBarOrdersByStatus(Status status)
        {
            List<Order> orders = new List<Order>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * " +
                    "FROM [Order] " +
                    "WHERE orderID IN (" +
                    "SELECT orderID FROM OrderItem JOIN MenuItem ON OrderItem.itemID = MenuItem.itemID " +
                    "WHERE item_type = 'Drink' AND [status] = '@status') " +
                    "ORDER BY orderTime;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", status.ToString());
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Order order = ReadOrder(reader);
                    orders.Add(order);
                }
                reader.Close();
            }
            return orders;
        }

        public List<Order> GetKitchenOrdersByStatus(Status status)
        {
            List<Order> orders = new List<Order>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * " +
                    "FROM [Order] " +
                    "WHERE orderID IN (" +
                    "SELECT orderID FROM OrderItem JOIN MenuItem ON OrderItem.itemID = MenuItem.itemID " +
                    "WHERE item_type = 'Dish' AND OrderItem.[status] = @status) " +
                    "ORDER BY orderTime;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", status.ToString());
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Order order = ReadOrder(reader);
                    orders.Add(order);
                }
                reader.Close();
            }
            return orders;
        }

        private Order ReadOrder(SqlDataReader reader)
        {
            int orderId = (int)reader["orderID"];

            int employeeID = (int)reader["employeeID"];
            Employee employee = GetEmployeeByID(employeeID);

            int tableID = (int)reader["tableID"];
            Table table = GetTableByID(tableID);

            DateTime orderTime = (DateTime)reader["orderTime"];
            bool isServed = true;
            List<OrderItem> orderItems = GetOrderItemsByOrderID(orderId);

            return new Order(orderId, employee, table, orderTime, isServed, orderItems);
        }

        private Employee GetEmployeeByID (int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT employeeID, employee_name, employee_role " +
                    "FROM Employee " +
                    "WHERE employeeID = @Id; ";

                SqlCommand command = new SqlCommand(query, connection);

                // Preventing SQL injections
                command.Parameters.AddWithValue("@Id", id);

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (!reader.Read())
                    return null;
                Employee employee = ReadEmployee(reader);
                reader.Close();

                return employee;
            }
        }

        private Employee ReadEmployee(SqlDataReader reader)
        {
            int employeeId = (int)reader["employeeID"];
            string name = (string)reader["employee_name"];
            Role role = (Role)Enum.Parse(typeof(Role), (string)reader["employee_role"], true);

            return new Employee(employeeId, name, role);
        }

        private Table GetTableByID(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * " +
                    "FROM [Table] " +
                    "WHERE tableID = @Id; ";

                SqlCommand command = new SqlCommand(query, connection);

                // Preventing SQL injections
                command.Parameters.AddWithValue("@Id", id);

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (!reader.Read())
                    return null;
                Table table = ReadTable(reader);
                reader.Close();

                return table;
            }
        }
        private Table ReadTable(SqlDataReader reader)
        {
            int tableID = (int)reader["tableID"];
            int tableNumber = (int)reader["table_number"];
            bool isOccupied = (bool)reader["isOccupied"];

            return new Table(tableID, tableNumber, isOccupied);
        }

        private List<OrderItem> GetOrderItemsByOrderID(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                List<OrderItem> orderItems = new List<OrderItem>();
                string query = "SELECT itemID, includeDate, [status], quantity " +
                    "FROM OrderItem " +
                    "WHERE orderID = @Id; ";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    OrderItem orderItem = ReadOrderItem(reader);
                    orderItems.Add(orderItem);
                }
                reader.Close();
                return orderItems;
            }
        }

        private OrderItem ReadOrderItem(SqlDataReader reader)
        {
            int itemID = (int)reader["itemID"];
            MenuItem menuItem = GetMenuItemByID(itemID);

            DateTime includeDate = (DateTime)reader["includeDate"];
            Status status = (Status)Enum.Parse(typeof(Status), (string)reader["status"], true);
            //Status status = (Status)reader["status"];
            int quantity = (int)reader["quantity"];

            return new OrderItem(itemID, menuItem, includeDate, status, quantity);
        }

        public MenuItem GetMenuItemByID(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * " +
                    "FROM [MenuItem] " +
                    "WHERE itemID = @Id; ";

                SqlCommand command = new SqlCommand(query, connection);

                // Preventing SQL injections
                command.Parameters.AddWithValue("@Id", id);

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (!reader.Read())
                    return null;
                MenuItem menuItem = ReadMenuItem(reader);
                reader.Close();

                return menuItem;
            }
        }
        private MenuItem ReadMenuItem(SqlDataReader reader)
        {

            return new MenuItem
            {
                ItemID = (int)reader["itemID"],
                Item_name = reader["item_name"].ToString(),
                Description = reader["description"].ToString(),
                Price = (decimal)reader["price"],
                VATPercent = (decimal)reader["VATpercent"],
                Category = reader["category"].ToString(),
                StockQuantity = (int)reader["stockQuantity"]
            };
        }
        public Order GetOrderById(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM [Order] WHERE OrderID = @OrderId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Order
                        {
                            OrderID = (int)reader["OrderID"],
                            Table = new Table { TableID = (int)reader["TableID"] },
                            OrderItems = new List<OrderItem>(), // optional to load separately
                                                                // Add more properties as needed
                        };
                    }
                }
            }

            return null;
        }
        public void CloseOrder(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE [Order] SET Status = @Status WHERE OrderID = @OrderId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Status", Status.Served); // or Status.Closed, as per your enum
                command.Parameters.AddWithValue("@OrderId", orderId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }


    }
}

