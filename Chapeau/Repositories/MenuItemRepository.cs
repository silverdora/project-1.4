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
        public MenuItem GetMenuItemByID(int itemID)
        {
            string query = "SELECT * FROM MenuItem WHERE itemID = @itemID";
            SqlParameter parameter = new SqlParameter("@itemID", itemID);

            List<MenuItem> result = ExecuteQueryMapMenuItems(query, parameter);

            foreach (var item in result)
            {
                return item; // return the first one
            }
            return null; // if list is empty

        }

        // Get items by card name (e.g., "Lunch", "Dinner", "Drinks")
        public List<MenuItem> GetMenuItemsByCard(string cardName)
        {
            string query = @"
                SELECT mi.*
                FROM MenuItem mi
                JOIN MenuCard mc ON mi.cardID = mc.icardID
                WHERE mc.card_name = @card";

            SqlParameter cardParameter = new SqlParameter("@card", cardName);
            return ExecuteQueryMapMenuItems(query, cardParameter);
        }

        // Get items by category (e.g., "Starters")
        public List<MenuItem> GetMenuItemsByCategory(string categoryName)
        {
            string query = "SELECT * FROM MenuItem WHERE category = @category";

            SqlParameter categoryParameter = new SqlParameter("@category", categoryName);
            return ExecuteQueryMapMenuItems(query, categoryParameter);
        }

        //Get items by both card and category
        public List<MenuItem> GetMenuItemsByCardAndCategory(string cardName, string categoryName)
        {
            string query = @"
                SELECT mi.*
                FROM MenuItem mi
                JOIN MenuCard mc ON mi.cardID = mc.icardID
                WHERE mc.card_name = @card AND mi.category = @category";

            SqlParameter cardParameter = new SqlParameter("@card", cardName);
            SqlParameter categoryParameter = new SqlParameter("@category", categoryName);

            return ExecuteQueryMapMenuItems(query, cardParameter, categoryParameter);
        }

        // Shared helper
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