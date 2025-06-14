using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        void AddPayment(Payment payment);
        List<Payment> GetAllPayments();
        void MarkPaymentComplete(int paymentId);

        void Add(Payment payment);

        Order GetActiveOrderByTable(int tableId);
        Order GetOrderById(int orderId);



    }
}
