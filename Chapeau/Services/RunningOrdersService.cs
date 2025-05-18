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
    }
}

