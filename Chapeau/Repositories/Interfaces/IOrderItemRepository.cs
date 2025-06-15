using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IOrderItemRepository
    {
        void Insert(OrderItem item);
        OrderItem? GetByOrderAndItem(int orderId, int itemId);

        void UpdateQuantity(int orderId, int itemId, int newQuantity);        

    }
}
