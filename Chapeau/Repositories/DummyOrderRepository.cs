using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;
using Chapeau.Repository.Interface;

namespace Chapeau.Repositories
{
    public class DummyOrderRepository
    {
        private readonly string _connectionString;

        public DummyOrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public Order GetActiveOrderByTable(int tableId)
        {
            Order order = null;

            string query = @"
                SELECT o.orderID, o.tableID, o.isServed, o.orderTime, o.isPaid,
                       oi.itemID, oi.quantity, oi.includeDate, oi.status AS itemStatus,
                       m.itemID, m.item_name, m.price, m.VATPercent
                FROM [Order] o
                JOIN OrderItem oi ON o.orderID = oi.orderID
                JOIN MenuItem m ON oi.itemID = m.itemID
                WHERE o.tableID = @tableId AND o.isPaid = 0";



            using (SqlConnection conn = new SqlConnection(_connectionString))

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@tableId", tableId);
               

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (order == null)
                        {
                            order = new Order
                            {
                                OrderID = reader.GetInt32(0),
                                Table = new Table { TableNumber = reader.GetInt32(1) },
                                IsServed = reader.GetBoolean(2),
                                OrderTime = reader.GetDateTime(3),
                                Status = reader.GetBoolean(4) ? Status.Served : Status.New,
                                OrderItems = new List<OrderItem>()
                            };
                        }

                        MenuItem menuItem = new MenuItem
                        {
                            ItemID = reader.GetInt32(9),
                            Item_name = reader.GetString(10),
                            Price = reader.GetDecimal(11),
                            VATPercent = reader.GetDecimal(12)
                        };

                        OrderItem item = new OrderItem
                        {
                            OrderID = order.OrderID,
                            ItemID = reader.GetInt32(5),
                            Quantity = reader.GetInt32(6),
                            IncludeDate = reader.GetDateTime(7),
                            Status = (Status)Enum.Parse(typeof(Status), reader.GetString(8)),




                            MenuItem = menuItem
                        };

                        order.OrderItems.Add(item);
                    }
                }
            }

            return order;
        }

    }
}
