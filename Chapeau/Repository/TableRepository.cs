using Chapeau.Models;
using Chapeau.Repository.Interface;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repository
{
    public class TableRepository : ITableRepository
    {
        private readonly string _connectionString;

        public TableRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Table> GetAllTables()
        {
            List<Table> tables = new List<Table>();
            string query = "SELECT tableID, table_number, isOccupied FROM [Table]";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tables.Add(new Table
                        {
                            TableID = Convert.ToInt32(reader["tableID"]),
                            TableNumber = Convert.ToInt32(reader["table_number"]),
                            Status = Convert.ToBoolean(reader["isOccupied"]) ? TableStatus.Occupied : TableStatus.Available,  
                        });
                    }
                }
            }

            return tables;
        }
    }
}

