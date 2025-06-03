using System;
using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;
//using System.Data.SqlClient;

using Humanizer;
using System.Text.RegularExpressions;

namespace Chapeau.Repositories
{
	public class RunningOrdersRepository : IRunningOrdersRepository
	{
        private readonly string _connectionString;

        public RunningOrdersRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        

        public void ChangeOrderStatus(int orderID, int itemID, Status status)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"UPDATE OrderItem " +
                    $"SET status = @status " +
                    $"WHERE orderID = @orderId AND itemID = @itemId";

                SqlCommand command = new SqlCommand(query, connection);

                // Preventing SQL injections
                command.Parameters.AddWithValue("@orderId", orderID);
                command.Parameters.AddWithValue("@itemId", itemID);
                command.Parameters.AddWithValue("@status", status.ToString());

                command.Connection.Open();
                int nrOfRowsAffected = command.ExecuteNonQuery();
                if (nrOfRowsAffected != 1)
                {
                    throw new Exception("Changing status failed!");
                }
            }
        }

        public void ChangeAllOrderItemsStatus(int orderID, Status currentStatus, Status newStatus)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"UPDATE OrderItem " +
                    $"SET [status] = @newStatus " +
                    $"WHERE orderID = @orderId AND [status] = @currentStatus";

                SqlCommand command = new SqlCommand(query, connection);

                // Preventing SQL injections
                command.Parameters.AddWithValue("@orderId", orderID);
                command.Parameters.AddWithValue("@newStatus", newStatus.ToString());
                command.Parameters.AddWithValue("@currentStatus", currentStatus.ToString());

                command.Connection.Open();
                int nrOfRowsAffected = command.ExecuteNonQuery();
                if (nrOfRowsAffected == 0)
                {
                    throw new Exception("Changing status failed!");
                }
            }
        }

        public void ChangeOrderItemsFromOneCourseStatus(int orderID, Status currentStatus, Status newStatus, MenuCategory course)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"UPDATE OrderItem " +
                    $"SET [status] = @newStatus " +
                    $"WHERE itemID in ( " +
                    $"SELECT itemID " +
                    $"from MenuItem " +
                    $"where category = @course ) " +
                    $"AND orderID = @orderId AND [status] = @currentStatus;";

                SqlCommand command = new SqlCommand(query, connection);

                // Preventing SQL injections
                command.Parameters.AddWithValue("@orderId", orderID);
                command.Parameters.AddWithValue("@newStatus", newStatus.ToString());
                command.Parameters.AddWithValue("@currentStatus", currentStatus.ToString());
                command.Parameters.AddWithValue("@course", course.ToString());

                command.Connection.Open();
                int nrOfRowsAffected = command.ExecuteNonQuery();
                if (nrOfRowsAffected == 0)
                {
                    throw new Exception("Changing status failed!");
                }
            }
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
                    "WHERE item_type = 'Drink' AND [status] = @status) " +
                    "ORDER BY orderTime;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", status.ToString());
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Order order = ReadOrder(reader, status, "Drink");
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
                string query = "SELECT *" +
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
                    Order order = ReadOrder(reader, status, "Dish");
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
            Employee employee = GetEmployeeByID(employeeID);

            int tableID = (int)reader["tableID"];
            Table table = GetTableByID(tableID);

            DateTime orderTime = (DateTime)reader["orderTime"];
            //bool isServed = true;
            List<OrderItem> orderItems = GetOrderItemsByOrderID(orderId, status, type);

            return new Order(orderId, employee, table, orderTime, orderItems);

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
                string query = @"SELECT t.tableID, t.table_number, t.isOccupied,
                   MAX(oi.status) AS OrderStatus
                    FROM[Table] t
                    LEFT JOIN[Order] o ON t.tableID = o.tableID
                    LEFT JOIN[OrderItem] oi ON o.orderID = oi.orderID
                    WHERE t.tableID = @Id
                    GROUP BY t.tableID, t.table_number, t.isOccupied;
                ";

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
            Status? orderStatus = reader["orderStatus"] != DBNull.Value // Mo for sprint 2
                ? (Status)Enum.Parse(typeof(Status), (string)reader["orderStatus"], true)
                : null;

            return new Table(tableID, tableNumber, isOccupied, orderStatus);   // Mo for sprint 2
        }

        private List<OrderItem> GetOrderItemsByOrderID(int id, Status status, string type)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                List<OrderItem> orderItems = new List<OrderItem>();
                string query = "SELECT OrderItem.itemID, includeDate, [status], quantity " +
                    "FROM OrderItem JOIN MenuItem ON OrderItem.itemID = MenuItem.itemID " +
                    "WHERE orderID = @Id and [status] = @status and item_type = @type; ";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@status", status.ToString());
                command.Parameters.AddWithValue("@type", type);
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
            //string notes = (string)reader["notes"];

            return new OrderItem(itemID, menuItem, includeDate, status, quantity); //need to remove ItemID as it's now we are only using menyItem (MATHEUS)
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
                Category = (MenuCategory)Enum.Parse(typeof(MenuCategory), (string)reader["category"], true),
                StockQuantity = (int)reader["stockQuantity"]
            };
        }
    }
}

