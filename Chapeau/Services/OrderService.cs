using Chapeau.Models;
using Chapeau.Services.Interfaces;
using Chapeau.Repositories.Interfaces;

namespace Chapeau.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Order TakeNewOrder(int tableId, Employee employee)
        {
            return _orderRepository.TakeNewOrder(tableId, employee);
        }
       
        public Order GetOrderById(int orderID)
        {
           return _orderRepository.GetOrderByID(orderID);
        }
    }
}
