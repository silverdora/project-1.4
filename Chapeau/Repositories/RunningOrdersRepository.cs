using System;
using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using System.Data;
using System.Data.SqlClient;
using Chapeau.Enumerations;
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

        

        public void ChangeOrderStatus(OrderItem orderItem, int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"UPDATE OrderItem " +
                    $"SET status = @status " +
                    $"WHERE orderID = @Id";

                SqlCommand command = new SqlCommand(query, connection);

                // Preventing SQL injections
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@status", orderItem.Status);

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
                    "WHERE item_type = 'Drink' AND [status] != 'completed') " +
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
                    "WHERE item_type = 'Dish' AND [status] != 'completed') " +
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
                    "WHERE item_type = 'Drink' AND [status] != '@status') " +
                    "ORDER BY orderTime;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", status);
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
                    "WHERE item_type = 'Dish' AND [status] != '@status') " +
                    "ORDER BY orderTime;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", status);
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
            List<OrderItem> orderItems = new List<OrderItem>();

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
            Role role = (Role)reader["employee_role"];

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
            Status status = (Status)reader["status"];
            int quantity = (int)reader["quantity"];

            return new OrderItem(menuItem, includeDate, status, quantity);
        }

        private MenuItem GetMenuItemByID(int id)
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
            int itemID = (int)reader["itemID"];
            string item_name = (string)reader["item_name"];
            string description = (string)reader["description"];
            decimal price = (decimal)reader["price"];
            decimal VATPercent = (decimal)reader["VATPercent"];
            string category = (string)reader["category"];
            int stockQuantity = (int)reader["stockQuantity"];

            return new MenuItem(itemID, item_name, description, price, VATPercent, category, stockQuantity);
        }
    }
}

