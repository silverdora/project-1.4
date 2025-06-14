using Chapeau.Models;
using System.Security.Cryptography;

namespace Chapeau.Services
{
    public interface IOrderService
    {       
        //take an order
        void InsertOrder(Order order);
        void AddItemToSessionSelection(int menuItemId, int quantity, ISession session);
        List<OrderItem> GetSelectedItemsFromSession(ISession session);
        void AddItemsToOrder(int orderId, List<OrderItem> items);

        //kitchen or bar
        List<Order> GetOrdersByStatus(Status status, string type);
        void ChangeOrderItemStatus(int orderItemID, Status status);
        void ChangeAllOrderItemsStatus(int orderID, string type, Status currentStatus, Status newStatus);
        Dictionary<int, List<MenuCategory>> GetCategoriesOfAnOrder(List<Order> order);
        void ChangeOrderItemsFromOneCourseStatus(int orderID, Status currentStatus, Status newStatus, MenuCategory course);
    }
}
