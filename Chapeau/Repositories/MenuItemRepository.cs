using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using System.Data;
using System.Data.SqlClient;


namespace Chapeau.Repositories
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly string _connectionString;

        public MenuItemRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<MenuItem> GetAll()
        {
            List<MenuItem> items = new List<MenuItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM MenuItem";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new MenuItem
                            {
                                ItemID = (int)reader["ItemID"],
                                Item_name = reader["Item_name"].ToString(),
                                Description = reader["Description"].ToString(),
                                Price = (decimal)reader["Price"],
                                VATPercent = (decimal)reader["VATPercent"],
                                Category = reader["Category"].ToString(),
                                StockQuantity = (int)reader["StockQuantity"]
                            });
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Something went wrong with the database", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("Something went wrong reading user data", ex);
                }
            }

            return items;
        }
    }

    //public List<MenuItem> GetFiltered(string card, string category)
    //{
    //    var query = _context.MenuItems.AsQueryable();

    //    if (!string.IsNullOrEmpty(card) && card.ToLower() != "all")
    //    {
    //        query = query.Where(m => m.Card == card);
    //    }

    //    if (!string.IsNullOrEmpty(category) && category.ToLower() != "all")
    //    {
    //        query = query.Where(m => m.Category == category);
    //    }

    //    return query.ToList();
    //}
}

