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

       
        public List<MenuItem> GetAllMenuItems()
        {
            return ExecuteQueryMapMenuItems("SELECT * FROM MenuItem");
        }

       
        public List<MenuItem> GetMenuItemsByCard(MenuCard card)
        {
            string query = @"SELECT mi.*
                             FROM MenuItem mi
                             JOIN MenuCard mc ON mi.cardID = mc.icardID
                             WHERE mc.card_name = @card";

            var cardName = card.ToString(); 
            SqlParameter cardParameter = new SqlParameter("@card", cardName);
            return ExecuteQueryMapMenuItems(query, cardParameter);
        }

        
        public List<MenuItem> GetMenuItemsByCategory(MenuCategory category)
        {
            string query = "SELECT * FROM MenuItem WHERE category = @category";
            string categoryName = category.ToString(); 
            SqlParameter categoryParameter = new SqlParameter("@category", categoryName);
            return ExecuteQueryMapMenuItems(query, categoryParameter);
        }

        public List<MenuItem> GetMenuItemsByCardAndCategory(MenuCard card, MenuCategory category)
        {
            string query = @"SELECT mi.*
                             FROM MenuItem mi
                             JOIN MenuCard mc ON mi.cardID = mc.icardID
                             WHERE mc.card_name = @card AND mi.category = @category";

            SqlParameter cardParameter = new SqlParameter("@card", card.ToString());
            SqlParameter categoryParameter = new SqlParameter("@category", category.ToString());

            return ExecuteQueryMapMenuItems(query, cardParameter, categoryParameter); 
        }


        public void AddMenuItem(MenuItem item)
        {
            string query = @"INSERT INTO MenuItem 
                            (item_name, description, price, VATpercent, category, stockQuantity, cardID, item_type, IsActive) 
                             VALUES 
                            (@name, @description, @price, @vat, @category, @stock, @cardID, @itemType, @isActive)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", item.Item_name);
                cmd.Parameters.AddWithValue("@description", item.Description);
                cmd.Parameters.AddWithValue("@price", item.Price);
                cmd.Parameters.AddWithValue("@vat", item.VATPercent);
                cmd.Parameters.AddWithValue("@category", item.Category);
                cmd.Parameters.AddWithValue("@stock", item.StockQuantity);
                cmd.Parameters.AddWithValue("@cardID", item.CardID);
                cmd.Parameters.AddWithValue("@itemType", item.Item_Type);
                cmd.Parameters.AddWithValue("@isActive", item.IsActive);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateMenuItem(MenuItem item)
        {
            string query = @"UPDATE MenuItem 
                             SET item_name = @name, description = @description, price = @price, VATpercent = @vat,
                                 category = @category, stockQuantity = @stock, cardID = @cardID, item_type = @itemType
                             WHERE itemID = @id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", item.ItemID);
                cmd.Parameters.AddWithValue("@name", item.Item_name);
                cmd.Parameters.AddWithValue("@description", item.Description);
                cmd.Parameters.AddWithValue("@price", item.Price);
                cmd.Parameters.AddWithValue("@vat", item.VATPercent);
                cmd.Parameters.AddWithValue("@category", item.Category);
                cmd.Parameters.AddWithValue("@stock", item.StockQuantity);
                cmd.Parameters.AddWithValue("@cardID", item.CardID);
                cmd.Parameters.AddWithValue("@itemType", item.Item_Type);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void ToggleMenuItemActive(int id, bool isActive)
        {
            string query = "UPDATE MenuItem SET IsActive = @isActive WHERE itemID = @id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@isActive", isActive);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public MenuItem GetMenuItemById(int id)
        {
            string query = "SELECT * FROM MenuItem WHERE itemID = @id";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new MenuItem
                        {
                            ItemID = (int)reader["itemID"],
                            Item_name = reader["item_name"].ToString(),
                            Description = reader["description"].ToString(),
                            Price = (decimal)reader["price"],
                            VATPercent = (decimal)reader["VATpercent"],
                            Category = reader["category"].ToString(),
                            StockQuantity = (int)reader["stockQuantity"],
                            CardID = (int)reader["cardID"],
                            Item_Type = reader["item_type"].ToString(),
                            IsActive = (bool)reader["IsActive"]
                        };
                    }
                }
            }

            return null;
        }
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
                                StockQuantity = (int)reader["stockQuantity"],
                                CardID = (int)reader["cardID"],
                                Item_Type = reader["item_type"].ToString(),
                                IsActive = (bool)reader["IsActive"]
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
