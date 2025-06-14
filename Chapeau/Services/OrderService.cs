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

        //methods connected with taking an order

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

        //methods connected with bar or kitchen
        public void ChangeOrderStatus(int orderItemID, Status status)
        {
            _orderItemRepository.ChangeOrderItemStatus(orderItemID, status);
        }

        public void ChangeAllOrderItemsStatus(int orderID, string type, Status currentStatus, Status newStatus)
        {
            // ...foreach orderitem in orderitems in ordercard
            _orderItemRepository.ChangeAllOrderItemsStatus(orderID, type, currentStatus, newStatus);
        }

        public List<Order> GetOrdersByStatus(Status status, string type)
        {
            DateTime createdAfter = DateTime.Today;
            return _orderRepository.GetOrdersByStatus(status, type, createdAfter);
        }


        public Dictionary<int, List<MenuCategory>> GetCategoriesOfAnOrder(List<Order> orders)
        {
            Dictionary<int, List<MenuCategory>> categoriesByOrderId = new Dictionary<int, List<MenuCategory>>();

            List<MenuCategory> courses = new List<MenuCategory> { MenuCategory.Starters, MenuCategory.Mains, MenuCategory.Desserts, MenuCategory.Entremets, MenuCategory.Beer, MenuCategory.Wine, MenuCategory.Spirits, MenuCategory.Coffee, MenuCategory.Tea, MenuCategory.SoftDrink };
            foreach (MenuCategory course in courses)
            {
                foreach (Order order in orders)
                {
                    foreach (OrderItem orderItem in order.OrderItems)
                    {
                        if (orderItem.MenuItem.Category == course)
                        {
                            if (!categoriesByOrderId.ContainsKey(order.OrderID))
                            {
                                categoriesByOrderId.Add(order.OrderID, new List<MenuCategory> { course });
                            }
                            else
                            {
                                if (!categoriesByOrderId[order.OrderID].Contains(course))
                                {
                                    categoriesByOrderId[order.OrderID].Add(course);
                                }

                            }
                        }
                    }
                }
            }
            return categoriesByOrderId;
        }

        public void ChangeOrderItemsFromOneCourseStatus(int orderID, Status currentStatus, Status newStatus, MenuCategory course)
        {
            _orderItemRepository.ChangeOrderItemsFromOneCourseStatus(orderID, currentStatus, newStatus, course);
        }

    }
}
