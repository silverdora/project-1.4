using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Repository.Interface;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
using Microsoft.Data.SqlClient; 


namespace Chapeau.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ITableRepository _tableRepository;

        public PaymentService(IPaymentRepository paymentRepository, ITableRepository tableRepository)
        {
            _paymentRepository = paymentRepository;
            _tableRepository = tableRepository;
        }

        public List<Payment> GetAllPayments(int orderId)
        {
            try
            {
                return _paymentRepository.GetPaymentsByOrderId(orderId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve payments for order {orderId}", ex);
            }
        }

        public void AddPayment(Payment payment)
        {
            try
            {
                ValidatePayment(payment);
                _paymentRepository.AddPayment(payment);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to add payment", ex);
            }
        }

        public void CompletePayment(int paymentId)
        {
            try
            {
                _paymentRepository.MarkPaymentComplete(paymentId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to complete payment {paymentId}", ex);
            }
        }

        public void SavePayment(FinishOrderViewModel model)
        {
            try
            {
                var payment = CreatePaymentFromViewModel(model);
                ValidatePayment(payment);
                _paymentRepository.AddPayment(payment);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to save payment", ex);
            }
        }

        public void SaveIndividualPayment(int orderId, decimal amountPaid, decimal tipAmount, string paymentType, string feedback)
        {
            try
            {
                var payment = CreatePayment(orderId, amountPaid, tipAmount, paymentType, feedback);
                ValidatePayment(payment);
                _paymentRepository.AddPayment(payment);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to save individual payment", ex);
            }
        }

        public void MarkOrderAsPaid(int orderId)
        {
            try
            {
                _tableRepository.MarkOrderAsPaid(orderId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to mark order {orderId} as paid", ex);
            }
        }

        public int? GetLatestUnpaidOrderIdByTable(int tableId)
        {
            try
            {
                return _tableRepository.GetLatestUnpaidOrderIdByTable(tableId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get latest unpaid order for table {tableId}", ex);
            }
        }

        private Payment CreatePaymentFromViewModel(FinishOrderViewModel model)
        {
            return new Payment
            {
                orderID = model.OrderID,
                paymentType = model.PaymentType,
                amountPaid = model.AmountPaid,
                tipAmount = model.TipAmount,
                paymentDAte = DateTime.Now,
                lowVatAmount = model.LowVatAmount,
                highVATAmount = model.HighVatAmount,
                Feedback = model.Feedback
            };
        }

        private Payment CreatePayment(int orderId, decimal amountPaid, decimal tipAmount, string paymentType, string feedback)
        {
            return new Payment
            {
                orderID = orderId,
                amountPaid = amountPaid,
                tipAmount = tipAmount,
                paymentType = (PaymentType)Enum.Parse(typeof(PaymentType), paymentType, true),
                Feedback = feedback,
                paymentDAte = DateTime.Now
            };
        }

        private void ValidatePayment(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment), "Payment cannot be null");

            if (payment.orderID <= 0)
                throw new ArgumentException("Invalid order ID", nameof(payment.orderID));

            if (payment.amountPaid <= 0)
                throw new ArgumentException("Amount paid must be greater than zero", nameof(payment.amountPaid));

            if (payment.tipAmount < 0)
                throw new ArgumentException("Tip amount cannot be negative", nameof(payment.tipAmount));
        }
    }
}