using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        void AddPayment(Payment payment);
        List<Payment> GetAllPayments();
        List<Payment> GetPaymentsByOrderId(int orderId);
        void MarkPaymentComplete(int paymentId);
        Order GetActiveOrderByTable(int tableId);
        Order GetOrderById(int orderId);
    }
}

