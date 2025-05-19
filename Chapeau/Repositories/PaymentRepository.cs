using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly string _connectionString;

        public PaymentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public void AddPayment(Payment payment)
        {
            string query = "INSERT INTO Payment (paymentID, orderID, paymentType, amountPaid, tipAmount, paymentDAte, lowVATAmount, highVATAmount) " +
                              $"VALUES (@paymentID, @orderID, @paymentType, @amountPaid,  @tipAmount, @paymentDAte, @lowVATAmount, @highVATAmount)";


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@paymentID", payment.paymentID);
                    command.Parameters.AddWithValue("@orderID", payment.orderID);
                    command.Parameters.AddWithValue("@PaymentType", payment.paymentType);
                    command.Parameters.AddWithValue("@amountPaid", payment.amountPaid);
                    command.Parameters.AddWithValue("@tipAmount", payment.tipAmount);
                    command.Parameters.AddWithValue("@paymentDAte", payment.paymentDAte);
                    command.Parameters.AddWithValue("@lowVATAmount", payment.lowVATAmount);
                    command.Parameters.AddWithValue("@highVATAmount", payment.highVATAmount);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {

                        throw new Exception("Something went wrong");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Something went wrong");
                    }
                }
            }

        }
        public List<Payment> GetAllPayments()
        {
            List<Payment> payments = new List<Payment>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Payment";
                SqlCommand cmd = new SqlCommand(query, connection);
                try
                {
                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payments.Add(new Payment
                            {
                                paymentID = (int)reader["paymentID"],
                                orderID = (int)reader["orderID"],
                                paymentType = reader["paymentType"].ToString(),
                                amountPaid = (decimal)reader["amountPaid"],
                                tipAmount = (decimal)reader["tipAmount"],
                                paymentDAte = (DateTime)reader["paymentDAte"],
                                lowVATAmount = reader["lowVATAmount"] != DBNull.Value ? (decimal)reader["lowVATAmount"] : 0,
                                highVATAmount = reader["highVATAmount"] != DBNull.Value ? (decimal)reader["highVATAmount"] : 0
                            });
                        }
                    }
                }

                catch (Exception ex)
                {
                    throw new Exception("Something went wrong reading the data", ex);
                }
            }
            return payments;
        }

        public Payment GetPaymentById(int id)
        {
            // Implementation optional based on need
            throw new NotImplementedException();
        }

    }
}