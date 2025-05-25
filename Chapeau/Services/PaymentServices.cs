using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;

namespace Chapeau.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

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




    }
}
