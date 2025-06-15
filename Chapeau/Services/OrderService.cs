using Chapeau.Models;
using Chapeau.Repositories.Interfaces;

namespace Chapeau.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderService(IOrderRepository orderRepository, IMenuItemRepository menuItemRepository, IOrderItemRepository orderItemRepository)
        {
            _orderRepository = orderRepository;
            _menuItemRepository = menuItemRepository;
            _orderItemRepository = orderItemRepository;
        }
        public Order? GetActiveOrderByTableId(int tableId)
        {
            return _orderRepository.GetActiveOrderByTableId(tableId);
        }
        public Order GetOrCreateActiveOrder(int tableId, Employee employee)
        {
            Order? existingOrder = _orderRepository.GetActiveOrderByTableId(tableId);
            if (existingOrder != null)
                return existingOrder;

            Order newOrder = new Order
            {
                Table = new Table { TableId = tableId, IsOccupied = true },
                Employee = employee,
                OrderTime = DateTime.Now,
                IsPaid = false,
            };
            _orderRepository.InsertOrder(newOrder);
            return newOrder;
        }
        public void AddItemsToOrder(int orderId, List<OrderItem> items)
        {
            foreach (OrderItem item in items)
            {
                _orderItemRepository.Insert(item, orderId);
            }
        }
        public void FinalizeOrder(Order order)
        {
            foreach (OrderItem item in order.OrderItems)
            {
                _orderItemRepository.Insert(item, order.OrderId);
                _menuItemRepository.ReduceStock(item.MenuItem.ItemId, item.Quantity);
            }
        }
        public void InsertOrder(Order order)
        {
            // You could set the table as occupied here if needed
            _orderRepository.InsertOrder(order);
        }
    }
}