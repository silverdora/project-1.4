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





    }
}
