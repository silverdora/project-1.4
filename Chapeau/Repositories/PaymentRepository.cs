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
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public void AddPayment(Payment payment)
        {
            try
            {
                string query = @"
                    INSERT INTO Payment 
                    (orderID, paymentType, amountPaid, tipAmount, paymentDate, lowVatAmount, highVATAmount, Feedback)
                    VALUES (@orderID, @paymentType, @amountPaid, @tipAmount, @paymentDate, @lowVatAmount, @highVATAmount, @feedback)";

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
                    cmd.Parameters.AddWithValue("@feedback", string.IsNullOrEmpty(payment.Feedback) ? DBNull.Value : (object)payment.Feedback);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to add payment", ex);
            }
        }

        public List<Payment> GetAllPayments()
        {
            try
            {
                List<Payment> payments = new List<Payment>();
                string query = @"
                    SELECT p.paymentID, p.orderID, p.paymentType, p.amountPaid, p.tipAmount, 
                           p.paymentDate, p.lowVatAmount, p.highVATAmount, p.Feedback,
                           o.tableID, o.orderTime, o.isPaid
                    FROM Payment p
                    JOIN [Order] o ON p.orderID = o.orderID
                    ORDER BY p.paymentDate DESC";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payments.Add(MapPaymentFromReader(reader));
                        }
                    }
                }
                return payments;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve payments", ex);
            }
        }

        public List<Payment> GetPaymentsByOrderId(int orderId)
        {
            try
            {
                List<Payment> payments = new List<Payment>();
                string query = @"
                    SELECT p.paymentID, p.orderID, p.paymentType, p.amountPaid, p.tipAmount, 
                           p.paymentDate, p.lowVatAmount, p.highVATAmount, p.Feedback,
                           o.tableID, o.orderTime, o.isPaid
                    FROM Payment p
                    JOIN [Order] o ON p.orderID = o.orderID
                    WHERE p.orderID = @orderId
                    ORDER BY p.paymentDate DESC";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payments.Add(MapPaymentFromReader(reader));
                        }
                    }
                }
                return payments;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve payments for order {orderId}", ex);
            }
        }

        public void MarkPaymentComplete(int paymentId)
        {
            try
            {
                string query = @"
                    UPDATE Payment 
                    SET isComplete = 1 
                    WHERE paymentID = @paymentID";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@paymentID", paymentId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to mark payment {paymentId} as complete", ex);
            }
        }

        public Order GetActiveOrderByTable(int tableId)
        {
            try
            {
                string query = @"
                    SELECT o.orderID, o.tableID, o.orderTime, o.isPaid,
                           oi.itemID, oi.quantity, oi.includeDate, oi.status AS itemStatus,
                           m.itemID, m.item_name, m.price, m.VATPercent
                    FROM [Order] o
                    JOIN OrderItem oi ON o.orderID = oi.orderID
                    JOIN MenuItem m ON oi.itemID = m.itemID
                    WHERE o.tableID = @tableId 
                    AND o.isPaid = 0
                    ORDER BY o.orderTime DESC";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@tableId", tableId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        return MapOrderFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve active order for table {tableId}", ex);
            }
        }

        public Order GetOrderById(int orderId)
        {
            try
            {
                string query = @"
                    SELECT o.orderID, o.tableID, o.orderTime, o.isPaid,
                           oi.itemID, oi.quantity, oi.includeDate, oi.status AS itemStatus,
                           m.itemID, m.item_name, m.price, m.VATPercent
                    FROM [Order] o
                    JOIN OrderItem oi ON o.orderID = oi.orderID
                    JOIN MenuItem m ON oi.itemID = m.itemID
                    WHERE o.orderID = @orderId";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        return MapOrderFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve order {orderId}", ex);
            }
        }

        private Payment MapPaymentFromReader(SqlDataReader reader)
        {
            return new Payment
            {
                paymentID = reader.GetInt32(0),
                orderID = reader.GetInt32(1),
                paymentType = (PaymentType)Enum.Parse(typeof(PaymentType), reader.GetString(2)),
                amountPaid = reader.GetDecimal(3),
                tipAmount = reader.GetDecimal(4),
                paymentDAte = reader.GetDateTime(5),
                lowVatAmount = reader.GetDecimal(6),
                highVATAmount = reader.GetDecimal(7),
                Feedback = reader.IsDBNull(8) ? null : reader.GetString(8)
            };
        }

        private Order MapOrderFromReader(SqlDataReader reader)
        {
            Order order = null;
            int itemCount = 0;
            while (reader.Read())
            {
                if (order == null)
                {
                    order = new Order
                    {
                        OrderID = reader.GetInt32(0),
                        Table = new Table { TableId = reader.GetInt32(1) },
                        OrderTime = reader.GetDateTime(2),
                        Status = reader.GetBoolean(3) ? Status.Served : Status.Ordered,
                        OrderItems = new List<OrderItem>()
                    };
                }

                MenuItem menuItem = new MenuItem
                {
                    ItemID = reader.GetInt32(8),
                    Item_name = reader.GetString(9),
                    Price = reader.GetDecimal(10),
                    VATPercent = reader.GetDecimal(11)
                };

                OrderItem item = new OrderItem
                {
                    ItemID = reader.GetInt32(4),
                    Quantity = reader.GetInt32(5),
                    IncludeDate = reader.GetDateTime(6),
                    Status = (Status)Enum.Parse(typeof(Status), reader.GetString(7), true),
                    MenuItem = menuItem
                };

                order.OrderItems.Add(item);
                itemCount++;
            }
            if (order == null)
                throw new Exception("MapOrderFromReader: No rows returned from SQL query.");
            if (itemCount == 0)
                throw new Exception("MapOrderFromReader: Order found but no items were mapped.");
            return order;
        }
    }
}