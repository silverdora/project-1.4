using System;
using Chapeau.Models;

namespace Chapeau.Services
{
	public interface IRunningOrdersService
	{
        
        List<Order> GetOrdersByStatus(Status status, string type);
        void ChangeOrderStatus(int orderItemID, Status status);
        void ChangeAllOrderItemsStatus(int orderID, string type, Status currentStatus, Status newStatus);
        Dictionary<int, List<MenuCategory>> GetCategoriesOfAnOrder(List<Order> order);
        void ChangeOrderItemsFromOneCourseStatus(int orderID, Status currentStatus, Status newStatus, MenuCategory course);
    }
}


