using Chapeau.Models;
using Chapeau.HelperMethods;
using Chapeau.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
        public Order? GetActiveOrderByTableId(int tableId)
        {
            return _orderRepository.GetActiveOrderByTableId(tableId);

        }

        // stores the selected menu item into the session list.
        public void AddItemToSessionSelection(int menuItemId, int quantity, ISession session)
        {
            // Get the current list from session or create a new one
            List<OrderItem> selectedItems = session.GetSelectedItemsFromSession();         
            OrderItem existingItem = null;

            // Search for existing item
            foreach (OrderItem item in selectedItems)
            {
                if (item.MenuItem.ItemID == menuItemId)
                {
                    existingItem = item;
                    break;
                }
            }

            // If item exists, increase quantity
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                // If it doesn't exist, retrieve the menu item and add a new one
                MenuItem menuItem = _menuItemRepository.GetMenuItemByID(menuItemId);
                OrderItem newItem = new OrderItem(null, menuItem, DateTime.Now, Status.Ordered, quantity, null);
                selectedItems.Add(newItem);
            }

            // Save back to session
            session.SetObjectAsJson("SelectedItems", selectedItems);
        }

        public void AddItemsToOrder(int orderId, List<OrderItem> items)
        {
            foreach (var item in items)
            {
                //item.OrderID = orderId;
                _orderItemRepository.Insert(item, orderId);
            }
        }      

        //public void ClearSelectedItemsFromSession(ISession session)
        //{
        //    session.Remove("SelectedItems");
        //}       

    }
}
