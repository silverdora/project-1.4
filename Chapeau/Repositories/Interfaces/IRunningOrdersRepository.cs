using System;
using Chapeau.Models;
namespace Chapeau.Repositories.Interfaces
{
    public interface IRunningOrdersRepository
    {
        List<Order> GetOrdersByStatus(Status status, string type, DateTime createdAfter);
        void ChangeOrderItemStatus(int orderItemID, Status status);
        void ChangeAllOrderItemsStatus(int orderID, string type, Status currentStatus, Status newStatus);
        void ChangeOrderItemsFromOneCourseStatus(int orderID, Status currentStatus, Status newStatus, MenuCategory course);
        //List<Order> GetServedOrders();
    }
}

