using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;

namespace Chapeau.Services
{
    public class DummyOrderService: IDummyOrderService
    {
        private readonly IDummyOrderRepository _orderRepository;
        private static List<Order> _orders = new List<Order>();

        public DummyOrderService(IDummyOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _orderRepository.GetAllOrders();
        }

        public Order GetOrderById(int id)
        {
            return _orderRepository.GetOrderById(id);
        }

        public void AddOrder(Order order)
        {
            order.OrderID = _orders.Count + 1; // auto-increment ID
            _orders.Add(order);
        }

        public void UpdateOrderStatus(int orderId, Status status)
        {
            _orderRepository.UpdateOrderStatus(orderId, status);
        }

    }

}

