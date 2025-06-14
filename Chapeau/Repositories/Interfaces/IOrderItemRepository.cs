using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IOrderItemRepository
    {
        void Insert(OrderItem item, int orderId);
    }
}
