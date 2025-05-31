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

        public void Insert(OrderItem item)
        {
            string query = @"INSERT INTO OrderItem (orderID, itemID, includeDate, status, quantity)
                             VALUES (@orderID, @itemID, @includeDate, @status, @quantity)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@orderID", item.OrderID);
                command.Parameters.AddWithValue("@itemID", item.MenuItem.ItemID);
                command.Parameters.AddWithValue("@includeDate", item.OrderDateTime);
                command.Parameters.AddWithValue("@status", item.Status.ToString());
                command.Parameters.AddWithValue("@quantity", item.Quantity);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}

