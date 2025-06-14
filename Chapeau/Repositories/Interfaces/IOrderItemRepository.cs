using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IOrderItemRepository
    {
        //taking an order
        void Insert(OrderItem item, int orderId);

        //bar or kitchen
        void ChangeOrderItemStatus(int orderItemID, Status status);
        void ChangeAllOrderItemsStatus(int orderID, string type, Status currentStatus, Status newStatus);
        void ChangeOrderItemsFromOneCourseStatus(int orderID, Status currentStatus, Status newStatus, MenuCategory course);
        List<OrderItem> GetOrderItemsByOrderID(int id, Status status, string type);
    }
}
