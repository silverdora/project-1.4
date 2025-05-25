using System;
using Chapeau.Models;
namespace Chapeau.Repositories.Interfaces
{
    public interface IRunningOrdersRepository
    {
        //OrderItem GetOrderItemByID(int id);
        List<Order> GetAllBarOrders();
        List<Order> GetAllKitchenOrders();
        List<Order> GetBarOrdersByStatus(Status status);
        List<Order> GetKitchenOrdersByStatus(Status status);
        void ChangeOrderStatus(int itemID, Status status);

        Order GetOrderById(int orderID);
        void MarkOrderAsCompleted(int orderID);

    }
}

