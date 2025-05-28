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
            string query = "SELECT tableID, table_number, isOccupied AS is_occupied FROM [Table]";

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
                            TableId = Convert.ToInt32(reader["tableID"]),
                            TableNumber = Convert.ToInt32(reader["table_number"]),
                            IsOccupied = Convert.ToBoolean(reader["is_occupied"])
                        });
                    }
                }
            }

            return tables;
        }
        public List<Table> GetTablesWithOrderStatus()
        {
            List<Table> tables = new List<Table>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
    SELECT t.tableID, t.table_number, t.isOccupied,
           MAX(oi.status) AS OrderStatus
    FROM [Table] t
    LEFT JOIN [Order] o ON t.tableID = o.tableID
    LEFT JOIN [OrderItem] oi ON o.orderID = oi.orderID
    GROUP BY t.tableID, t.table_number, t.isOccupied
";


                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Table table = new Table
                    {
                        TableId = Convert.ToInt32(reader["tableID"]),
                        TableNumber = Convert.ToInt32(reader["table_number"]),
                        IsOccupied = Convert.ToBoolean(reader["isOccupied"]),
                        OrderStatus = reader["OrderStatus"] != DBNull.Value
                            ? Enum.TryParse<Status>(reader["OrderStatus"].ToString(), out var status) ? status : null
                            : null

                    };

                    tables.Add(table);
                }
            }

            return tables;
        }


    }
}

