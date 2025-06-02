using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        void AddPayment(Payment payment);
        List<Payment> GetAllPayments();
        void MarkPaymentComplete(int paymentId);
        // ... other methods you need


    }
}
