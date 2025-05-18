using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;
using static Chapeau.HelperMethods.MenuItemFilters;


namespace Chapeau.Repositories
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly string _connectionString;

        public MenuItemRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Get all menu items
        public List<MenuItem> GetAllMenuItems()
        {
            return ExecuteQueryMapMenuItems("SELECT * FROM MenuItem");
        }

        // Get items by card name
        public List<MenuItem> GetMenuItemsByCard(MenuCard card)
        {
            string query = @"SELECT mi.*
                             FROM MenuItem mi
                             JOIN MenuCard mc ON mi.cardID = mc.icardID
                             WHERE mc.card_name = @card";

            var cardName = card.ToString(); // converting enum to string for SQL 

            return ExecuteQueryMapMenuItems(query, new SqlParameter("@card", cardName));
        }

        // Get items by category
        public List<MenuItem> GetMenuItemsByCategory(MenuCategory category)
        {
            string query = "SELECT * FROM MenuItem WHERE category = @category";
            string categoryName = category.ToString(); // converting enum to string for SQL 
            return ExecuteQueryMapMenuItems(query, new SqlParameter("@category", categoryName));
        }

        public List<MenuItem> GetMenuItemsByCardAndCategory(MenuCard card, MenuCategory category)
        {
            string query = @"SELECT mi.*
                             FROM MenuItem mi
                            JOIN MenuCard mc ON mi.cardID = mc.icardID
                            WHERE mc.card_name = @card AND mi.category = @category";

            return ExecuteQueryMapMenuItems(query,
                new SqlParameter("@card", card.ToString()),
                new SqlParameter("@category", category.ToString()));
        }


        // Shared helper to reduce duplication
        private List<MenuItem> ExecuteQueryMapMenuItems(string query, params SqlParameter[] parameters)
        {
            List<MenuItem> items = new List<MenuItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters);

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