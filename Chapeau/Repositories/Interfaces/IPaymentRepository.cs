using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        List<Payment> GetAllPayments();
        Payment GetPaymentById(int id);
        void AddPayment(Payment payment);
        void MarkPaymentComplete(int id);
        void CompletePayment(Payment payment);


    }
}
