using Chapeau.Models;
using System.Security.Cryptography;

namespace Chapeau.Services
{
    public interface IOrderService
    {       
        void InsertOrder(Order order);
        void FinalizeOrder(Order order);
        Order? GetActiveOrderByTableId(int tableId);
        Order GetOrCreateActiveOrder(int tableId, Employee employee);
        void AddItemsToOrder(int orderId, List<OrderItem> items);



    }
}
