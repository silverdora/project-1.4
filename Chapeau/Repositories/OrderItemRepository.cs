using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly string _connectionString;

        public OrderItemRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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
    
        public OrderItem? GetByOrderAndItem(int orderId, int itemId)
        {
            using var conn = new SqlConnection(_connectionString);
            string query = @"SELECT * FROM OrderItem WHERE orderID = @orderId AND itemID = @itemId";
            var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@orderId", orderId);
            cmd.Parameters.AddWithValue("@itemId", itemId);

            conn.Open();
            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new OrderItem
                {
                    OrderID = orderId,
                    ItemID = itemId,
                    IncludeDate = Convert.ToDateTime(reader["includeDate"]),
                    Status = Enum.Parse<Status>(reader["status"].ToString()),
                    Quantity = Convert.ToInt32(reader["quantity"]),
                    Comment = reader["comment"] as string
                };
            }

            return null;
        }

        public void UpdateQuantity(int orderId, int itemId, int newQuantity)
        {
            using var conn = new SqlConnection(_connectionString);
            string query = @"UPDATE OrderItem SET quantity = @quantity WHERE orderID = @orderId AND itemID = @itemId";
            var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@quantity", newQuantity);
            cmd.Parameters.AddWithValue("@orderId", orderId);
            cmd.Parameters.AddWithValue("@itemId", itemId);

            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

}