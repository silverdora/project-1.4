using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Order TakeNewOrder(int tableId, Employee employee);
        Order GetOrderByID(int orderID);
        bool OrderItemExists(int orderID, int itemID);
        void InsertOrderItem(int orderID, OrderItem item);
        void UpdateOrderItem(int orderID, OrderItem item);


    }
}
