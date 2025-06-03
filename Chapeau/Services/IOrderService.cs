using Chapeau.Models;
using System.Security.Cryptography;

namespace Chapeau.Services
{
    public interface IOrderService
    {       
        void InsertOrder(Order order);

        void AddItemToSessionSelection(int menuItemId, int quantity, ISession session);

        List<OrderItem> GetSelectedItemsFromSession(ISession session);

        void AddItemsToOrder(int orderId, List<OrderItem> items);

    }
}
