using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;
using Chapeau.Exceptions;

namespace Chapeau.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly string _connectionString;
        private IMenuItemRepository _menuItemRepository;

        public OrderItemRepository(IConfiguration configuration, IMenuItemRepository menuItemRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _menuItemRepository = menuItemRepository;
        }

        public void Insert(OrderItem item, int orderId)
        {
            string query = @"INSERT INTO OrderItem (orderItemID, orderID, itemID, includeDate, status, quantity)
                             VALUES (@orderItemID, @orderID, @itemID, @includeDate, @status, @quantity)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@orderItemID", item.OrderItemID);
                command.Parameters.AddWithValue("@orderID", orderId);
                command.Parameters.AddWithValue("@itemID", item.MenuItem.ItemID);
                command.Parameters.AddWithValue("@includeDate", item.IncludeDate);
                command.Parameters.AddWithValue("@status", item.Status.ToString());
                command.Parameters.AddWithValue("@quantity", item.Quantity);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<OrderItem> GetOrderItemsByOrderID(int id, Status status, string type)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                List<OrderItem> orderItems = new List<OrderItem>();
                string query = "SELECT OrderItem.orderItemID, OrderItem.itemID, includeDate, [status], quantity, comment  " +
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
            int orderItemID = (int)reader["orderItemID"];
            int itemID = (int)reader["itemID"];
            MenuItem menuItem = _menuItemRepository.GetMenuItemByID(itemID);

            DateTime includeDate = (DateTime)reader["includeDate"];
            Status status = (Status)Enum.Parse(typeof(Status), (string)reader["status"], true);
            int quantity = (int)reader["quantity"];
            string? comment = null;
            if (!(reader["comment"] == null || reader["comment"] == DBNull.Value))
            {
                comment = (string)reader["comment"];
            }
            return new OrderItem(orderItemID, menuItem, includeDate, status, quantity, comment);
        }

        public void ChangeOrderItemStatus(int orderItemID, Status status)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"UPDATE OrderItem " +
                    $"SET status = @status " +
                    $"WHERE orderItemID = @orderItemID";

                SqlCommand command = new SqlCommand(query, connection);

                // Preventing SQL injections
                command.Parameters.AddWithValue("@orderItemID", orderItemID);
                command.Parameters.AddWithValue("@status", status.ToString());

                command.Connection.Open();
                int nrOfRowsAffected = command.ExecuteNonQuery();
                if (nrOfRowsAffected != 1)
                {
                    throw new ChangeStatusException();
                }
            }
        }

        public void ChangeAllOrderItemsStatus(int orderID, string type, Status currentStatus, Status newStatus)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"UPDATE OrderItem " +
                    $"SET [status] = @newStatus " +
                    $"WHERE itemID in ( " +
                    $"SELECT itemID from MenuItem where item_type = @type )" +
                    $"AND orderID = @orderId AND [status] = @currentStatus";

                SqlCommand command = new SqlCommand(query, connection);

                // Preventing SQL injections
                command.Parameters.AddWithValue("@orderId", orderID);
                command.Parameters.AddWithValue("@type", type);
                command.Parameters.AddWithValue("@newStatus", newStatus.ToString());
                command.Parameters.AddWithValue("@currentStatus", currentStatus.ToString());

                command.Connection.Open();
                int nrOfRowsAffected = command.ExecuteNonQuery();
                if (nrOfRowsAffected == 0)
                {
                    throw new ChangeStatusException();
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
                    throw new ChangeStatusException();
                }
            }
        }
    }
}

