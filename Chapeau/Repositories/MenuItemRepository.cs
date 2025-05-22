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

        // Get all menu items
        public List<MenuItem> GetMenuItems()
        {
            return ExecuteQueryMapMenuItems("SELECT * FROM MenuItem");
        }

        // Get items by card
        public List<MenuItem> GetMenuItemsByCard(MenuCard card)
        {
            string query = @"SELECT mi.*
                             FROM MenuItem mi
                             JOIN MenuCard mc ON mi.cardID = mc.icardID
                             WHERE mc.card_name = @card";

            var cardName = card.ToString(); // converting enum to string for SQL 

            SqlParameter cardParameter = new SqlParameter("@card", cardName);//to replace @card in the query
            return ExecuteQueryMapMenuItems(query, cardParameter);
        }

        // Get items by category
        public List<MenuItem> GetMenuItemsByCategory(MenuCategory category)
        {
            string query = "SELECT * FROM MenuItem WHERE category = @category";
            string categoryName = category.ToString(); // converting enum to string for SQL 

            SqlParameter categoryParameter = new SqlParameter("@category", categoryName);//to replace @category in the query
            return ExecuteQueryMapMenuItems(query, categoryParameter);  
        }

        public List<MenuItem> GetMenuItemsByCardAndCategory(MenuCard card, MenuCategory category)
        {
            string query = @"SELECT mi.*
                             FROM MenuItem mi
                            JOIN MenuCard mc ON mi.cardID = mc.icardID
                            WHERE mc.card_name = @card AND mi.category = @category";

            //SQL parameters
            SqlParameter cardParameter = new SqlParameter("@card", card.ToString());
            SqlParameter categoryParameter = new SqlParameter("@category", category.ToString());

            return ExecuteQueryMapMenuItems(query, cardParameter, categoryParameter);    // Call the helper method with both parameters
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
                    try
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
                    catch (SqlException ex)
                    {
                        throw new Exception("Database error while loading menu item.", ex);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error mapping menu item from result.", ex);
                    }
                }
            }
            return items;
        }
    }
}