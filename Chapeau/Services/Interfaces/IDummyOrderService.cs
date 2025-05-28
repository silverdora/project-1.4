using Chapeau.Models;

namespace Chapeau.Services.Interfaces
{
    public interface IDummyOrderService
    {
        IEnumerable<Order> GetAllOrders();
        Order GetOrderById(int id);
        void AddOrder(Order order);
        void UpdateOrderStatus(int orderId, Status status);
    }
}
