using Chapeau.Models;
using Chapeau.Services.Interfaces;
using Chapeau.Repositories.Interfaces;
using Chapeau.Repositories;
using Chapeau.HelperMethods;
using Microsoft.AspNetCore.Mvc;

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
        public void InsertOrder(Order order)
        {
            _orderRepository.InsertOrder(order);
        }

        // stores the selected menu item into the session list.
        public void AddItemToSessionSelection(int menuItemId, int quantity, ISession session)
        {
            // Retrieve the current list from session (or create a new one if it doesn't exist)
            List<OrderItem> selectedItems = session.GetObjectFromJson<List<OrderItem>>("SelectedItems");
            if (selectedItems == null)
            {
                selectedItems = new List<OrderItem>();
            }

            OrderItem existingItem = null;
            foreach (OrderItem item in selectedItems)
            {
                if (item.MenuItem.ItemID == menuItemId)
                {
                    existingItem = item;
                    break;
                }
            }

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                MenuItem menuItem = _menuItemRepository.GetMenuItemByID(menuItemId);
                OrderItem newItem = new OrderItem(menuItem.ItemID, menuItem, DateTime.Now, Status.Ordered, quantity, null);
                selectedItems.Add(newItem);
            }

            session.SetObjectAsJson("SelectedItems", selectedItems);
        }

        // Inserts the items from session into the database, using OrderItemRepository.
        public void AddItemsToOrder(int orderId, List<OrderItem> items)
        {
            foreach (var item in items)
            {
                item.OrderID = orderId;
                _orderItemRepository.Insert(item);
            }
        }

        public List<OrderItem> GetSelectedItemsFromSession(ISession session)
        {
            return session.GetObjectFromJson<List<OrderItem>>("SelectedItems") ?? new List<OrderItem>();
        }

        //public void ClearSelectedItemsFromSession(ISession session)
        //{
        //    session.Remove("SelectedItems");
        //}       

    }
}
