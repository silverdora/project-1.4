using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
using Microsoft.Data.SqlClient;

namespace Chapeau.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly string _connectionString;
        private readonly IPaymentRepository _paymentRepository;
        private readonly DummyOrderService _orderService;

        public PaymentService(IPaymentRepository paymentRepository, DummyOrderService orderService)
        {
            _paymentRepository = paymentRepository;
            _orderService = orderService;
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
            _paymentRepository.AddPayment(payment);
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
                    cmd.Parameters.AddWithValue("@paymentType", model.PaymentType.ToString());
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

        public void SaveIndividualPayment(int orderId, decimal amountPaid, decimal tipAmount, PaymentType paymentType, string feedback)
        {
            try
            { 
                if (orderId <= 0)
                    throw new ArgumentException("Invalid order ID");

                if (amountPaid <= 0)
                    throw new ArgumentException("Amount paid must be greater than 0");

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
            catch (Exception ex)
            {
                // Log the error
                throw new Exception($"Failed to save individual payment: {ex.Message}", ex);
            }
        }
    }
}

