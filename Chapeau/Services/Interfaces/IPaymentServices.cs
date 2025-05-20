using Chapeau.Models;

namespace Chapeau.Services.Interfaces
{
    public interface IPaymentService
    {
        List<Payment> GetAllPayments(int orderId);
        void ProcessPaymentForOrder(Order order, string paymentType, decimal tip);

        // ...
    }
}