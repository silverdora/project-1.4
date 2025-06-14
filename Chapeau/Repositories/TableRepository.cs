using Chapeau.Models;
using Chapeau.Repository.Interface;
using Chapeau.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

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

        public List<TableOrderViewModel> GetTableOverview()
        {
            var tableOverview = new List<TableOrderViewModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT 
                t.tableID, t.table_number, t.isOccupied,
                MAX(CASE WHEN mi.category = 'Drinks' THEN oi.status END) AS DrinkStatus,
                MAX(CASE WHEN mi.category <> 'Drinks' THEN oi.status END) AS FoodStatus
            FROM [Table] t
            LEFT JOIN [Order] o ON t.tableID = o.tableID
            LEFT JOIN [OrderItem] oi ON o.orderID = oi.orderID
            LEFT JOIN [MenuItem] mi ON oi.itemID = mi.itemID
            GROUP BY t.tableID, t.table_number, t.isOccupied";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var viewModel = new TableOrderViewModel
                    {
                        TableId = Convert.ToInt32(reader["tableID"]),
                        TableNumber = Convert.ToInt32(reader["table_number"]),
                        IsOccupied = Convert.ToBoolean(reader["isOccupied"]),
                        DrinkStatus = reader["DrinkStatus"] != DBNull.Value && Enum.TryParse(reader["DrinkStatus"].ToString(), out Status drinkStatus)
                            ? drinkStatus
                            : (Status?)null,
                        FoodStatus = reader["FoodStatus"] != DBNull.Value && Enum.TryParse(reader["FoodStatus"].ToString(), out Status foodStatus)
                            ? foodStatus
                            : (Status?)null
                    };

                    tableOverview.Add(viewModel);
                }
            }

            return tableOverview;
        }

        public List<TableOrderViewModel> GetTableOrderViewModels()
        {
            var viewModels = new List<TableOrderViewModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT 
                t.tableID, t.table_number, t.isOccupied,
                MAX(CASE WHEN mi.category = 'Drinks' THEN oi.status END) AS DrinkStatus,
                MAX(CASE WHEN mi.category <> 'Drinks' THEN oi.status END) AS FoodStatus
            FROM [Table] t
            LEFT JOIN [Order] o ON t.tableID = o.tableID
            LEFT JOIN [OrderItem] oi ON o.orderID = oi.orderID
            LEFT JOIN [MenuItem] mi ON oi.itemID = mi.itemID
            GROUP BY t.tableID, t.table_number, t.isOccupied";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var viewModel = new TableOrderViewModel
                    {
                        TableId = Convert.ToInt32(reader["tableID"]),
                        TableNumber = Convert.ToInt32(reader["table_number"]),
                        IsOccupied = Convert.ToBoolean(reader["isOccupied"]),
                        DrinkStatus = reader["DrinkStatus"] != DBNull.Value && Enum.TryParse(reader["DrinkStatus"].ToString(), out Status drinkStatus) ? drinkStatus : (Status?)null,
                        FoodStatus = reader["FoodStatus"] != DBNull.Value && Enum.TryParse(reader["FoodStatus"].ToString(), out Status foodStatus) ? foodStatus : (Status?)null
                    };

                    viewModels.Add(viewModel);
                }
            }

            return viewModels;
        }

        public Table GetTableByID(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT t.tableID, t.table_number, t.isOccupied,
                   MAX(oi.status) AS OrderStatus
                    FROM[Table] t
                    LEFT JOIN[Order] o ON t.tableID = o.tableID
                    LEFT JOIN[OrderItem] oi ON o.orderID = oi.orderID
                    WHERE t.tableID = @Id
                    GROUP BY t.tableID, t.table_number, t.isOccupied;
                ";

                SqlCommand command = new SqlCommand(query, connection);

                // Preventing SQL injections
                command.Parameters.AddWithValue("@Id", id);

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (!reader.Read())
                    return null;
                Table table = ReadTable(reader);
                reader.Close();

                return table;
            }
        }

        private Table ReadTable(SqlDataReader reader)
        {
            int tableID = (int)reader["tableID"];
            int tableNumber = (int)reader["table_number"];
            bool isOccupied = (bool)reader["isOccupied"];
            Status? orderStatus = reader["orderStatus"] != DBNull.Value // Mo for sprint 2
                ? (Status)Enum.Parse(typeof(Status), (string)reader["orderStatus"], true)
                : null;

            return new Table(tableID, tableNumber, isOccupied, orderStatus);   // Mo for sprint 2
        }

        // scnario 2 sprint 3
        public void UpdateTableOccupiedStatus(int tableId, bool isOccupied)
        {
            using var conn = new SqlConnection(_connectionString);
            string query = "UPDATE [Table] SET isOccupied = @isOccupied WHERE tableID = @tableId";
            var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@isOccupied", isOccupied);
            cmd.Parameters.AddWithValue("@tableId", tableId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public bool HasUnservedOrders(int tableId)
        {
            using var conn = new SqlConnection(_connectionString);
            string query = @"
        SELECT COUNT(*) 
        FROM [OrderItem] oi
        JOIN [Order] o ON o.orderID = oi.orderID
        WHERE o.tableID = @tableId AND oi.status != 'Served'";
            var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@tableId", tableId);
            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        public void MarkReadyOrdersAsServed(int tableId)
        {
            using var conn = new SqlConnection(_connectionString);
            string query = @"
        UPDATE oi
        SET status = 'Served'
        FROM [OrderItem] oi
        JOIN [Order] o ON o.orderID = oi.orderID
        WHERE o.tableID = @tableId AND oi.status = 'Ready'";
            var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@tableId", tableId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void MarkTableFreeByOrder(int orderId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                UPDATE [Table]
                SET IsOccupied = 0
                WHERE TableID = (SELECT TableID FROM [Order] WHERE OrderID = @orderId)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}