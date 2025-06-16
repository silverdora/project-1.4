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

            foreach (MenuItem item in result)
            {
                return item; // return the first one
            }
            return null; // if list is empty

        }

        // Get items by card name (e.g., "Lunch", "Dinner", "Drinks")
        public List<MenuItem> GetMenuItemsByCard(MenuCard card)
        {
            string query = "SELECT * FROM MenuItem WHERE card = @card";

            SqlParameter cardParameter = new SqlParameter("@card", card.ToString());
            return ExecuteQueryMapMenuItems(query, cardParameter);
        }

        // Get items by category (e.g., "Starters")
        public List<MenuItem> GetMenuItemsByCategory(MenuCategory category)
        {
            string query = "SELECT * FROM MenuItem WHERE category = @category";

            SqlParameter categoryParameter = new SqlParameter("@category", category.ToString());
            return ExecuteQueryMapMenuItems(query, categoryParameter);
        }

        //Get items by both card and category
        public List<MenuItem> GetMenuItemsByCardAndCategory(MenuCard card, MenuCategory category)
        {
            string query = "SELECT * FROM MenuItem WHERE card = @card AND category = @category";

            SqlParameter cardParameter = new SqlParameter("@card", card.ToString());
            SqlParameter categoryParameter = new SqlParameter("@category", category.ToString());

            return ExecuteQueryMapMenuItems(query, cardParameter, categoryParameter);
        }
        //method to reduce stock based in item order request
        public void ReduceStock(int itemId, int amount)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    string query = "UPDATE MenuItem SET stockQuantity = stockQuantity - @amount WHERE itemID = @itemId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@itemId", itemId);
                    command.Parameters.AddWithValue("@amount", amount);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Database error while reducing stock.", ex);
                }
            }
        }
        // Shared helper methods

        private MenuItem MapMenuItem(SqlDataReader reader)
        {
            return new MenuItem
            {
                ItemId = (int)reader["itemID"],
                Item_name = reader["item_name"].ToString(),
                Description = reader["description"].ToString(),
                Price = (decimal)reader["price"],
                VATPercent = (decimal)reader["VATpercent"],
                Category = (MenuCategory)Enum.Parse(typeof(MenuCategory), reader["category"].ToString(), true),
                StockQuantity = (int)reader["stockQuantity"],
                Card = reader["card"].ToString()
            };
        }  
        //using an array to allow me to pass many numbers of parameters
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
                            items.Add(MapMenuItem(reader));
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