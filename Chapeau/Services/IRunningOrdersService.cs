using System;
using Chapeau.Models;

namespace Chapeau.Services
{
	public interface IRunningOrdersService
	{
        
        List<Order> GetBarOrdersByStatus(Status status);
        List<Order> GetKitchenOrdersByStatus(Status status);
        void ChangeOrderStatus(int orderID, int itemID, Status status);
        void ChangeAllOrderItemsStatus(int orderID, Status currentStatus, Status newStatus);
        Dictionary<int, List<MenuCategory>> GetCategoriesOfAnOrder(List<Order> order);
    }
}


