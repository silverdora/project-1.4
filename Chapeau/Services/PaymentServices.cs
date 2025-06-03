using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
using Microsoft.Data.SqlClient; 


namespace Chapeau.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly string _connectionString;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public List<Payment> GetAllPayments(int orderID)
        {
            return _paymentRepository.GetAllPayments();
        }

        public void AddPayment(Payment payment)
        {
            _paymentRepository.AddPayment(payment);
        }
        public void CompletePayment(int id)
        {
            _paymentRepository.MarkPaymentComplete(id);
        }

        // Save or update the full payment details
        public void CompletePayment(Payment payment)
        {
            if (payment == null) return;
            // Either add new payment or update existing one, depending on your logic
            _paymentRepository.AddPayment(payment);
            // or _paymentRepository.UpdatePayment(payment); if you have update logic
        }

        public void SavePayment(FinishOrderViewModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO Payment (orderID, paymentType, amountPaid, tipAmount, paymentDAte, lowVatAmount, highVATAmount)
                         VALUES (@orderID, @paymentType, @amountPaid, @tipAmount, @paymentDate, @lowVatAmount, @highVATAmount)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@orderID", model.OrderID);
                    cmd.Parameters.AddWithValue("@paymentType", model.PaymentType);
                    cmd.Parameters.AddWithValue("@amountPaid", model.AmountPaid);
                    cmd.Parameters.AddWithValue("@tipAmount", model.TipAmount);
                    cmd.Parameters.AddWithValue("@paymentDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@lowVatAmount", model.LowVatAmount);
                    cmd.Parameters.AddWithValue("@highVATAmount", model.HighVatAmount);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

        }
        public void SaveIndividualPayment(int orderId, decimal amountPaid, decimal tipAmount, string paymentType, string feedback)
        {
            // Save payment record for the order into the database
            // Assuming you have a PaymentRepository or similar to persist data

            var payment = new Payment
            {
                orderID = orderId,
                amountPaid = amountPaid,
                tipAmount = tipAmount,
                paymentType = paymentType,
                Feedback = feedback,
                paymentDAte = DateTime.Now
            };

            _paymentRepository.Add(payment);
        }
    }
}
