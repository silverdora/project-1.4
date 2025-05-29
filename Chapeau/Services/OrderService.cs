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

        public void AddSingleItemToOrder(int orderID, int itemID, int quantity)
        {
            Order order = _orderRepository.GetOrderByID(orderID);
            if (order == null)
            {
                throw new Exception($"Order with ID {orderID} not found.");
            }

            List<MenuItem> allItems = _menuItemRepository.GetMenuItems();
            MenuItem item = null;

            // Find the menu item by ID
            foreach (MenuItem mi in allItems)
            {
                if (mi.ItemID == itemID)
                {
                    item = mi;
                    break;
                }
            }

            if (item == null)
            {
                throw new Exception("Menu item not found.");
            }

            // Check if the item already exists in the order
            OrderItem existingItem = null;
            foreach (OrderItem oi in order.OrderItems)
            {
                if (oi.ItemID == itemID)
                {
                    existingItem = oi;
                    break;
                }
            }

            if (existingItem != null)
            {
                existingItem.Quantity += quantity; // ✅ Use passed quantity
            }
            else
            {
                OrderItem newItem = new OrderItem(
                    itemID,
                    item,
                    DateTime.Now,
                    Status.New,
                    quantity // ✅ Use passed quantity
                );
                order.OrderItems.Add(newItem);
            }

            _orderRepository.UpdateOrderItems(order);
        }

        public void UpdateOrderItems(Order order)
        {
            _orderRepository.UpdateOrderItems(order);
        }

    }
}
