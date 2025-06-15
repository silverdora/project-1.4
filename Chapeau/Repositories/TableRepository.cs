// Required namespaces for data access, configuration, models, and view models
using Chapeau.Models;
using Chapeau.Repository.Interface;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System;
using Chapeau.HelperMethods;

namespace Chapeau.Repository
{
    // This repository handles all data access logic related to tables in the restaurant
    public class TableRepository : ITableRepository
    {
        private readonly string _connectionString;

        // Constructor that retrieves the connection string from app configuration
        public TableRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Retrieves all tables with basic information (ID, number, occupancy)
        public List<Table> GetAllTables()
        {
            List<Table> tables = new List<Table>();
            string query = "SELECT tableID, table_number, isOccupied AS is_occupied FROM [Table]";

            try
            {
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
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error retrieving tables.", ex);
            }

            return tables;
        }
        //(matheus)
        public Table? GetTableById(int tableId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT tableID, table_number, isOccupied FROM [Table] WHERE tableID = @tableID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@tableID", tableId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Table
                    {
                        TableId = (int)reader["tableID"],
                        TableNumber = (int)reader["table_number"],
                        IsOccupied = (bool)reader["isOccupied"]
                    };
                }

                return null; // Table not found
            }
        }




        // Retrieves tables along with the status of their orders (if any)
        public List<Table> GetTablesWithOrderStatus()
        {
            List<Table> tables = new List<Table>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = @"
                        SELECT t.tableID, t.table_number, t.isOccupied,
                               MAX(oi.status) AS OrderStatus
                        FROM [Table] t
                        LEFT JOIN [Order] o ON t.tableID = o.tableID
                        LEFT JOIN [OrderItem] oi ON o.orderID = oi.orderID
                        GROUP BY t.tableID, t.table_number, t.isOccupied";

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
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error retrieving tables with status.", ex);
            }

            return tables;
        }

        // Retrieves a detailed view model for each table including drink and food status and items
        public List<TableOrderViewModel> GetTableOverview()
        {
            var tableOverview = new List<TableOrderViewModel>();

            try
            {
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
                GROUP BY t.tableID, t.table_number, t.isOccupied;";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var viewModel = MapTableHelper.MapFromReader(reader);
                        int tableId = viewModel.TableId;

                        // Separate food and drink items for the view model
                        var allItems = GetOrderItemsByTable(tableId);
                        viewModel.FoodItems = allItems.Where(i => i.ItemType == "Dish").ToList();
                        viewModel.DrinkItems = allItems.Where(i => i.ItemType == "Drink").ToList();
                        tableOverview.Add(viewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error retrieving table overview.", ex);
            }

            return tableOverview;
        }

        // Maps a SQL row into a TableOrderViewModel object
        private TableOrderViewModel MapTableOrderViewModel(SqlDataReader reader)
        {
            return new TableOrderViewModel
            {
                TableId = Convert.ToInt32(reader["tableID"]),
                TableNumber = Convert.ToInt32(reader["table_number"]),
                IsOccupied = Convert.ToBoolean(reader["isOccupied"]),
                DrinkStatus = reader["DrinkStatus"] != DBNull.Value && Enum.TryParse(reader["DrinkStatus"].ToString(), out Status drinkStatus) ? drinkStatus : (Status?)null,
                FoodStatus = reader["FoodStatus"] != DBNull.Value && Enum.TryParse(reader["FoodStatus"].ToString(), out Status foodStatus) ? foodStatus : (Status?)null
            };
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

        // Updates the occupied status of a given table

        public void UpdateTableOccupiedStatus(int tableId, bool isOccupied)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                string query = "UPDATE [Table] SET isOccupied = @isOccupied WHERE tableID = @tableId";
                var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@isOccupied", isOccupied);
                cmd.Parameters.AddWithValue("@tableId", tableId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error updating table occupied status.", ex);
            }
        }

        // Checks whether a table has any unpaid orders
        public bool HasUnservedOrders(int tableId)
        {
            try
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
            catch (Exception ex)
            {
                throw new ApplicationException("Error checking unserved orders.", ex);
            }
        }

        // Marks all "ReadyToBeServed" order items for a table as "Served"
        public void MarkReadyOrdersAsServed(int tableId)
        {
            try
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
            catch (Exception ex)
            {
                throw new ApplicationException("Error marking orders as served.", ex);
            }
        }

        // Frees a table by marking it not occupied based on a given order ID
        public void MarkTableFreeByOrder(int orderId)
        {
            try
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
            catch (Exception ex)
            {
                throw new ApplicationException("Error marking table as free.", ex);
            }
        }

        // Marks an order as paid in the database
        public void MarkOrderAsPaid(int orderId)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                string query = "UPDATE [Order] SET IsPaid = 1 WHERE OrderID = @orderId";
                var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@orderId", orderId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error marking order as paid.", ex);
            }
        }

        // Retrieves the most recent unpaid order ID for a given table
        public int? GetLatestUnpaidOrderIdByTable(int tableId)
        {
            try
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
            catch (Exception ex)
            {
                throw new ApplicationException("Error getting latest unpaid order ID.", ex);
            }
        }

        // Retrieves all unpaid order items (food or drink) for a specific table
        public List<OrderItemViewModel> GetOrderItemsByTable(int tableId)
        {
            var items = new List<OrderItemViewModel>();

            try
            {
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
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error getting order items by table.", ex);
            }

            return items;
        }
    }
}
