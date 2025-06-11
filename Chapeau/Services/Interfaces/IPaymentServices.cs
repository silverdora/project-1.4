using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Services.Interfaces
{
    public interface IPaymentService
    {
        List<Payment> GetAllPayments(int orderID);
        void AddPayment(Payment payment);
        void CompletePayment(int id);
        void CompletePayment(Payment payment);
        void SavePayment(FinishOrderViewModel model);

        void SaveIndividualPayment(int orderId, decimal amountPaid, decimal tipAmount, PaymentType paymentType, string feedback);

    }
}