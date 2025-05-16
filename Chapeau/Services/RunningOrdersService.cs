using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using System;

namespace Chapeau.Services
{
	public class RunningOrdersService:IRunningOrdersService
	{
		public RunningOrdersService()
		{
		}

        public void ChangeOrderStatus(OrderItem orderItem, int id)
        {
            throw new NotImplementedException();
        }

        public List<Order> GetAllRunningOrders()
        {
            throw new NotImplementedException();
        }

        public List<Order> GetOrdersByStatus(string status)
        {
            throw new NotImplementedException();
        }
    }
}

