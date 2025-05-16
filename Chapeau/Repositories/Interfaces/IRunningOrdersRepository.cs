using System;
using Chapeau.Models;
namespace Chapeau.Repositories.Interfaces
{
	public interface IRunningOrdersRepository
	{
		List<Order> GetAllRunningOrders();
        List<Order> GetOrdersByStatus(string status);
        void ChangeOrderStatus(OrderItem orderItem, int id);
	}
}

