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
                cmd.Parameters.AddWithValue("@paymentType", payment.paymentType);
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
                            paymentType = reader.GetString(2),
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
    }
}
