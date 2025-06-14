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

        //methods connected with taking an order

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
                OrderItem newItem = new OrderItem(null, menuItem, DateTime.Now, Status.Ordered, quantity, null);
                selectedItems.Add(newItem);
            }

            session.SetObjectAsJson("SelectedItems", selectedItems);
        }

        // Inserts the items from session into the database, using OrderItemRepository.
        public void AddItemsToOrder(int orderId, List<OrderItem> items)
        {
            foreach (var item in items)
            {
                //item.OrderID = orderId;
                _orderItemRepository.Insert(item, orderId);
            }
        }

        public List<OrderItem> GetSelectedItemsFromSession(ISession session)
        {
            return session.GetObjectFromJson<List<OrderItem>>("SelectedItems") ?? new List<OrderItem>();
        }

        //methods connected with bar or kitchen
        public void ChangeOrderItemStatus(int orderItemID, Status status)
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
