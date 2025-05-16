using System;
using Chapeau.Models;

namespace Chapeau.Services
{
	public interface IRunningOrdersService
	{
        List<Order> GetAllRunningOrders();
        List<Order> GetOrdersByStatus(string status);
        void ChangeOrderStatus(OrderItem orderItem, int id);
    }
}


