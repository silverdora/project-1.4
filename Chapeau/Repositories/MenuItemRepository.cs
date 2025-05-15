using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;


namespace Chapeau.Repositories
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly string _connectionString;

        public MenuItemRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //public List<string> GetAllCards()
        //{
        //    List<string> cards = new List<string>();
        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        string query = "SELECT DISTINCT card_name FROM MenuCard";
        //        SqlCommand command = new SqlCommand(query, connection);
        //        connection.Open();
        //        using (SqlDataReader reader = command.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                cards.Add(reader["card_name"].ToString());
        //            }
        //        }
        //    }
        //    return cards;
        //}

        //public List<string> GetAllCategories()
        //{
        //    List<string> categories = new List<string>();
        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        string query = "SELECT DISTINCT category FROM MenuItem";
        //        SqlCommand command = new SqlCommand(query, connection);
        //        connection.Open();
        //        using (SqlDataReader reader = command.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                categories.Add(reader["category"].ToString());
        //            }
        //        }
        //    }
        //    return categories;
        //}

        public List<MenuItem> GetByCardAndCategory(string card, string category)
        {
            List<MenuItem> items = new List<MenuItem>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @" SELECT mi.* FROM MenuItem mi
                               JOIN BelongsTo bt ON mi.itemID = bt.itemID
                               JOIN MenuCard mc ON bt.icardID = mc.icardID
                               WHERE (@card = 'All' OR mc.card_name = @card)
                               AND (@category = 'All' OR mi.category = @category)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@card", card);
                command.Parameters.AddWithValue("@category", category);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new MenuItem
                        {
                            ItemID = (int)reader["itemID"],
                            Item_name = reader["item_name"].ToString(),
                            Description = reader["description"].ToString(),
                            Price = (decimal)reader["price"],
                            VATPercent = (decimal)reader["VATpercent"],
                            Category = reader["category"].ToString(),
                            StockQuantity = (int)reader["stockQuantity"]
                        });
                    }
                }
            }
            return items;
        }
    }
    
}