using Chapeau.Models;
using System.Security.Cryptography;

namespace Chapeau.Services
{
    public interface IOrderService
    {       
        void InsertOrder(Order order);

        Order? GetActiveOrderByTableId(int tableId);


       // void AddItemToSessionSelection(MenuItem menuItem, int quantity, ISession session);

        void AddItemsToOrder(int orderId, List<OrderItem> items);



    }
}
