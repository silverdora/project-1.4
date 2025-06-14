using Chapeau.Models;
using Chapeau.Repository.Interface;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System;

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
        WITH LatestOrderPerTable AS (
            SELECT o.orderID, o.tableID
            FROM [Order] o
            WHERE o.isPaid = 0
              AND o.orderID IN (
                  SELECT TOP 1 o2.orderID
                  FROM [Order] o2
                  WHERE o2.tableID = o.tableID
                  ORDER BY o2.orderTime DESC
              )
        )
        SELECT 
            t.tableID, 
            t.table_number, 
            t.isOccupied,
            MAX(CASE WHEN mi.item_type = 'Drink' THEN oi.status END) AS DrinkStatus,
            MAX(CASE WHEN mi.item_type = 'Dish' THEN oi.status END) AS FoodStatus
        FROM [Table] t
        LEFT JOIN LatestOrderPerTable lot ON lot.tableID = t.tableID
        LEFT JOIN [OrderItem] oi ON lot.orderID = oi.orderID
        LEFT JOIN [MenuItem] mi ON oi.itemID = mi.itemID
        GROUP BY t.tableID, t.table_number, t.isOccupied;
        ";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    // Existing viewModel creation
                    var viewModel = new TableOrderViewModel
                    {
                        TableId = Convert.ToInt32(reader["tableID"]),
                        TableNumber = Convert.ToInt32(reader["table_number"]),
                        IsOccupied = Convert.ToBoolean(reader["isOccupied"]),
                        DrinkStatus = reader["DrinkStatus"] != DBNull.Value
                                      && Enum.TryParse(reader["DrinkStatus"].ToString(), out Status drinkStatus)
                                      ? drinkStatus
                                      : (Status?)null,
                        FoodStatus = reader["FoodStatus"] != DBNull.Value
                                      && Enum.TryParse(reader["FoodStatus"].ToString(), out Status foodStatus)
                                      ? foodStatus
                                      : (Status?)null
                    };

                    // ✅ NEW: Load detailed items for this table
                    int tableId = viewModel.TableId;
                    var allItems = GetOrderItemsByTable(tableId); // <- this is the new helper method
                    viewModel.FoodItems = allItems.Where(i => i.ItemType == "Dish").ToList();
                    viewModel.DrinkItems = allItems.Where(i => i.ItemType == "Drink").ToList();

                    tableOverview.Add(viewModel);

                }
            }

            return tableOverview;
        }


        //public List<TableOrderViewModel> GetTableOrderViewModels()
        //{
        //    var viewModels = new List<TableOrderViewModel>();

        //    using (SqlConnection conn = new SqlConnection(_connectionString))
        //    {
        //        string query = @"
        //    SELECT 
        //        t.tableID, t.table_number, t.isOccupied,
        //        MAX(CASE WHEN mi.category = 'Drinks' THEN oi.status END) AS DrinkStatus,
        //        MAX(CASE WHEN mi.category <> 'Drinks' THEN oi.status END) AS FoodStatus
        //    FROM [Table] t
        //    LEFT JOIN [Order] o ON t.tableID = o.tableID
        //    LEFT JOIN [OrderItem] oi ON o.orderID = oi.orderID
        //    LEFT JOIN [MenuItem] mi ON oi.itemID = mi.itemID
        //    GROUP BY t.tableID, t.table_number, t.isOccupied";

        //        SqlCommand cmd = new SqlCommand(query, conn);
        //        conn.Open();
        //        SqlDataReader reader = cmd.ExecuteReader();

        //        while (reader.Read())
        //        {
        //            var viewModel = new TableOrderViewModel
        //            {
        //                TableId = Convert.ToInt32(reader["tableID"]),
        //                TableNumber = Convert.ToInt32(reader["table_number"]),
        //                IsOccupied = Convert.ToBoolean(reader["isOccupied"]),
        //                DrinkStatus = reader["DrinkStatus"] != DBNull.Value && Enum.TryParse(reader["DrinkStatus"].ToString(), out Status drinkStatus) ? drinkStatus : (Status?)null,
        //                FoodStatus = reader["FoodStatus"] != DBNull.Value && Enum.TryParse(reader["FoodStatus"].ToString(), out Status foodStatus) ? foodStatus : (Status?)null
        //            };

        //            viewModels.Add(viewModel);
        //        }
        //    }

        //    return viewModels;
        //}

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
        FROM [Order]
        WHERE tableID = @tableId AND IsPaid = 0";

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
        FROM OrderItem oi
        JOIN [Order] o ON o.orderID = oi.orderID
        WHERE o.tableID = @tableId
        AND oi.status = 'ReadyToBeServed';";

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

        // Lwana could make use of it?
        public void MarkOrderAsPaid(int orderId)
        {
            using var conn = new SqlConnection(_connectionString);
            string query = "UPDATE [Order] SET IsPaid = 1 WHERE OrderID = @orderId";
            var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@orderId", orderId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        //Mo.....sprint 3
        public int? GetLatestUnpaidOrderIdByTable(int tableId)
        {
            using var conn = new SqlConnection(_connectionString);
            string query = @"
        SELECT TOP 1 orderID
        FROM [Order]
        WHERE tableID = @tableId AND IsPaid = 0
        ORDER BY orderTime DESC";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@tableId", tableId);
            conn.Open();
            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : (int?)null;
        }


        //to show the items on the Table 

        public List<OrderItemViewModel> GetOrderItemsByTable(int tableId)
        {
            var items = new List<OrderItemViewModel>();

            using var conn = new SqlConnection(_connectionString);
            string query = @"
    SELECT mi.item_name, mi.item_type, oi.status, oi.quantity, mi.price, mi.VATPercent
    FROM [OrderItem] oi
    JOIN [Order] o ON oi.orderID = o.orderID
    JOIN [MenuItem] mi ON oi.itemID = mi.itemID
    WHERE o.tableID = @tableId AND o.IsPaid = 0";


            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@tableId", tableId);
            conn.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new OrderItemViewModel
                {
                    ItemName = reader["item_name"].ToString(),
                    ItemType = reader["item_type"].ToString(),
                    Status = Enum.Parse<Status>(reader["status"].ToString()),
                    Quantity = Convert.ToInt32(reader["quantity"]),
                    UnitPrice = Convert.ToDecimal(reader["price"]),
                    VATRate = Convert.ToDecimal(reader["VATPercent"])
                });


            }

            return items;
        }

    }
}