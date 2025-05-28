using System;
using Chapeau.Models;

namespace Chapeau.Services
{
	public interface IRunningOrdersService
	{
        List<Order> GetAllBarOrders();
        List<Order> GetAllKitchenOrders();
        List<Order> GetBarOrdersByStatus(Status status);
        List<Order> GetKitchenOrdersByStatus(Status status);
        void ChangeOrderStatus(int itemID, Status status);
        Dictionary<int, List<MenuCategory>> GetCategoriesOfAnOrder(List<Order> order);
    }
}


