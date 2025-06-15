using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Services.Interfaces
{
    public interface IPaymentService
    {
        List<Payment> GetAllPayments(int orderId);
        void AddPayment(Payment payment);
        void CompletePayment(int paymentId);
        void SavePayment(FinishOrderViewModel model);
        void SaveIndividualPayment(int orderId, decimal amountPaid, decimal tipAmount, string paymentType, string feedback);
        void MarkOrderAsPaid(int orderId);
        int? GetLatestUnpaidOrderIdByTable(int tableId);
    }
}