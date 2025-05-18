using Chapeau.Models;

namespace Chapeau.Services.Interface
{
    public interface IPaymentService
    {
        List<Payment> GetAllPayments(int orderId);
        // ...
    }
}