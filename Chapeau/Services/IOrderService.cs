using Chapeau.Models;
using System.Security.Cryptography;

namespace Chapeau.Services
{
    public interface IOrderService
    {
        Order TakeNewOrder(int tableId, Employee employee);
        Order GetOrderById(int orderID);
        void AddSingleItemToOrder(int orderID, int itemID, int quantity);
        void UpdateOrderItems(Order order);

    }
}
