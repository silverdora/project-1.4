using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interface;

namespace Chapeau.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public List<Payment> GetAllPayments(int orderId)
        {
            return _paymentRepository.GetAllPayments();
        }

    }
}
