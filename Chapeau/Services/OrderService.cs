using Chapeau.Models;
using Chapeau.Services.Interfaces;
using Chapeau.Repositories.Interfaces;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMenuItemRepository _menuItemRepository;


        public OrderService(IOrderRepository orderRepository, IMenuItemRepository menuItemRepository)
        {
            _orderRepository = orderRepository;
            _menuItemRepository = menuItemRepository;
        }

        public Order TakeNewOrder(int tableId, Employee employee)
        {
            return _orderRepository.TakeNewOrder(tableId, employee);
        }

        public Order GetOrderById(int orderID)
        {
            return _orderRepository.GetOrderByID(orderID);
        }


        //add item to an existing order
        public void AddSingleItemToOrder(int orderID, int itemID, int quantity)
        {
            Order order = _orderRepository.GetOrderByID(orderID);
            if (order == null)
            {
                throw new Exception($"Order with ID {orderID} not found.");
            }

            //calling an item by ID
            MenuItem item = _menuItemRepository.GetMenuItemByID(itemID);

            if (item == null)
            {
                throw new Exception("Menu item not found.");
            }


            OrderItem newItem = new OrderItem(
            itemID,
            item,
            DateTime.Now,
            Status.New,
            quantity);

            AddOrUpdateOrderItem(order, newItem);
        }

        public void AddOrUpdateOrderItem(Order order, OrderItem newItem)
        {
            bool exists = _orderRepository.OrderItemExists(order.OrderID, newItem.ItemID);

            if (exists)
            {
                _orderRepository.UpdateOrderItem(order.OrderID, newItem);
            }
            else
            {
                _orderRepository.InsertOrderItem(order.OrderID, newItem);
            }
        }

    }
}
