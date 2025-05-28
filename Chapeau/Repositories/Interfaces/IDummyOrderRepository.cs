using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IDummyOrderRepository
    {
        IEnumerable<Order> GetAllOrders();
        Order GetOrderById(int id);
        void AddOrder(Order order);
        void UpdateOrderStatus(int orderId, Status status);

    }
}
