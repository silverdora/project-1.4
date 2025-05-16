using System;
using Chapeau.Models;
namespace Chapeau.Repositories.Interfaces
{
    public interface IRunningOrdersRepository
    {
        OrderItem GetOrderItemByID(int id);
        List<Order> GetAllBarOrders();
        List<Order> GetAllKitchenOrders();
        List<Order> GetBarOrdersByStatus(Status status);
        List<Order> GetKitchenOrdersByStatus(Status status);
        void ChangeOrderStatus(OrderItem orderItem, int id);
    }
}

