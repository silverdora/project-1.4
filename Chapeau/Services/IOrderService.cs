using Chapeau.Models;
using Chapeau.ViewModels;
using System.Security.Cryptography;

namespace Chapeau.Services
{
    public interface IOrderService
    {       
        //take an order
        void InsertOrder(Order order);
        //kitchen or bar
        List<Order> GetOrdersByStatus(Status status, string type);
        void ChangeOrderItemStatus(int orderItemID, Status status);
        void ChangeAllOrderItemsStatus(int orderID, string type, Status currentStatus, Status newStatus);
        Dictionary<int, List<MenuCategory>> GetCategoriesOfAnOrder(List<Order> order);
        void ChangeOrderItemsFromOneCourseStatus(int orderID, Status currentStatus, Status newStatus, MenuCategory course);

        void FinalizeOrder(Order order);
        Order? GetActiveOrderByTableId(int tableId);
        Order GetOrCreateActiveOrder(int tableId, Employee employee);
        void AddItemsToOrder(int orderId, List<OrderItem> items);
        // Methods from DummyOrderService
        OrderSummaryViewModel GetOrderSummaryById(int orderId);
        void MarkOrderAsPaid(int orderId);
        Order GetOrderById(int orderId);
        decimal GetOrderTotal(int orderId);
    }
}
