using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using System;

namespace Chapeau.Services
{
	public class RunningOrdersService:IRunningOrdersService
	{
        private readonly IRunningOrdersRepository _runningOrdersRepository;

        public RunningOrdersService(IRunningOrdersRepository runningOrdersRepository)
        {
            _runningOrdersRepository = runningOrdersRepository;
        }

        public void ChangeOrderStatus(OrderItem orderItem, int id)
        {
            _runningOrdersRepository.ChangeOrderStatus(orderItem, id);
        }

        public List<Order> GetAllBarOrders()
        {
            return _runningOrdersRepository.GetAllBarOrders();
        }

        public List<Order> GetAllKitchenOrders()
        {
            return _runningOrdersRepository.GetAllKitchenOrders();
        }

        public List<Order> GetBarOrdersByStatus(string status)
        {
            return _runningOrdersRepository.GetBarOrdersByStatus(status);
        }

        public List<Order> GetKitchenOrdersByStatus(string status)
        {
            return _runningOrdersRepository.GetKitchenOrdersByStatus(status);
        }
    }
}

