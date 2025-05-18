using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        List<Payment> GetAllPayments();
        Payment GetPaymentById(int id);
        void AddPayment(Payment payment);
    }
}
