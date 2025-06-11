using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public PaymentRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection"); // make sure your connection string name matches
        }

        public void AddPayment(Payment payment)
        {
            string query = @"
                INSERT INTO Payment 
                (orderID, paymentType, amountPaid, tipAmount, paymentDAte, lowVatAmount, highVATAmount)
                VALUES (@orderID, @paymentType, @amountPaid, @tipAmount, @paymentDate, @lowVatAmount, @highVATAmount)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@orderID", payment.orderID);
                cmd.Parameters.AddWithValue("@paymentType", payment.paymentType.ToString());
                cmd.Parameters.AddWithValue("@amountPaid", payment.amountPaid);
                cmd.Parameters.AddWithValue("@tipAmount", payment.tipAmount);
                cmd.Parameters.AddWithValue("@paymentDate", payment.paymentDAte);
                cmd.Parameters.AddWithValue("@lowVatAmount", payment.lowVatAmount);
                cmd.Parameters.AddWithValue("@highVATAmount", payment.highVATAmount);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Payment> GetAllPayments()
        {
            List<Payment> payments = new List<Payment>();

            string query = "SELECT paymentID, orderID, paymentType, amountPaid, tipAmount, paymentDAte, lowVatAmount, highVATAmount FROM Payment";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        payments.Add(new Payment
                        {
                            paymentID = reader.GetInt32(0),
                            orderID = reader.GetInt32(1),
                            paymentType = (PaymentType)Enum.Parse(typeof(PaymentType), reader.GetString(2)),
                            amountPaid = reader.GetDecimal(3),
                            tipAmount = reader.GetDecimal(4),
                            paymentDAte = reader.GetDateTime(5),
                            lowVatAmount = reader.GetDecimal(6),
                            highVATAmount = reader.GetDecimal(7)
                        });
                    }
                }
            }
            return payments;
        }

        public void MarkPaymentComplete(int paymentId)
        {
            string query = "UPDATE Payment SET isComplete = 1 WHERE paymentID = @paymentID";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@paymentID", paymentId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void Add(Payment payment)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
            INSERT INTO Payments (orderID, amountPaid, tipAmount, paymentType, Feedback, paymentDAte)
            VALUES (@orderID, @amountPaid, @tipAmount, @paymentType, @Feedback, @paymentDAte)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@OrderID", payment.orderID);
                    cmd.Parameters.AddWithValue("@AmountPaid", payment.amountPaid);
                    cmd.Parameters.AddWithValue("@TipAmount", payment.tipAmount);
                    cmd.Parameters.AddWithValue("@PaymentType", payment.paymentType);
                    cmd.Parameters.AddWithValue("@Feedback", string.IsNullOrEmpty(payment.Feedback) ? DBNull.Value : (object)payment.Feedback);
                    cmd.Parameters.AddWithValue("@PaymentDate", payment.paymentDAte);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public Order GetActiveOrderByTable(int tableId)
        {
            Order order = null;

            string query = @"
                    SELECT o.orderID, o.tableID, o.isOccupied, o.orderTime, o.isPaid,
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
                                OrderTime = reader.GetDateTime(3),
                                Status = reader.GetBoolean(4) ? Status.Served : Status.Ordered,
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
        public Order GetOrderById(int orderId)
        {
            Order order = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT o.OrderID, o.OrderTime,
                           oi.ItemID, oi.Quantity,
                           m.Item_name, m.Price
                    FROM [Order] o
                    JOIN OrderItem oi ON o.OrderID = oi.OrderID
                    JOIN MenuItem m ON oi.ItemID = m.ItemID
                    WHERE o.OrderID = @orderId;";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@orderId", orderId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (order == null)
                        {
                            order = new Order
                            {
                                OrderID = orderId,
                                OrderTime = Convert.ToDateTime(reader["OrderTime"]),
                                OrderItems = new List<OrderItem>()
                            };
                        }

                        var orderItem = new OrderItem
                        {
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            MenuItem = new MenuItem
                            {
                                ItemID = Convert.ToInt32(reader["ItemID"]),
                                Item_name = reader["Item_name"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"])
                            }
                        };

                        order.OrderItems.Add(orderItem);
                    }
                }
            }

            return order;
        }
    }
}


