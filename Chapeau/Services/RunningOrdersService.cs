using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;
using System;

namespace Chapeau.Services
{
    public class RunningOrdersService : IRunningOrdersService
    {
        private readonly IRunningOrdersRepository _runningOrdersRepository;
        private readonly IPaymentService _paymentService;

        public RunningOrdersService(IRunningOrdersRepository runningOrdersRepository, IPaymentService paymentService)
        {
            _runningOrdersRepository = runningOrdersRepository;
            _paymentService = paymentService;
        }

        public void ChangeOrderStatus(int itemID, Status status)
        {
            _runningOrdersRepository.ChangeOrderStatus(itemID, status);
        }

        public List<Order> GetAllBarOrders()
        {
            return _runningOrdersRepository.GetAllBarOrders();
        }

        public List<Order> GetAllKitchenOrders()
        {
            return _runningOrdersRepository.GetAllKitchenOrders();
        }

        public List<Order> GetBarOrdersByStatus(Status status)
        {
            return _runningOrdersRepository.GetBarOrdersByStatus(status);
        }

        public List<Order> GetKitchenOrdersByStatus(Status status)
        {
            return _runningOrdersRepository.GetKitchenOrdersByStatus(status);
        }

        public void FinishOrder(int orderId, Payment payment)
        {
            CloseOrder(orderId);
            _paymentService.AddPayment(payment);
        }

        // Helper method to close order
        public void CloseOrder(int orderId)
        {
            _runningOrdersRepository.CloseOrder(orderId);
        }

        // Add your other methods (like GetCompleteOrderForTable) here as well
    }
}




