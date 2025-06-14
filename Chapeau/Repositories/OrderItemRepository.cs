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
            string query = @"INSERT INTO OrderItem (orderID, itemID, includeDate, status, quantity, comment)
                             VALUES (@orderID, @itemID, @includeDate, @status, @quantity, @comment)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@orderID", orderId);
                command.Parameters.AddWithValue("@itemID", item.MenuItem.ItemId);
                command.Parameters.AddWithValue("@includeDate", item.IncludeDate);
                command.Parameters.AddWithValue("@status", item.Status.ToString());
                command.Parameters.AddWithValue("@quantity", item.Quantity);
                command.Parameters.AddWithValue("@comment", (object?)item.Comment ?? DBNull.Value); //expects an obj


                connection.Open();
                command.ExecuteNonQuery();
            }
        }  
    }

}