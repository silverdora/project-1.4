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
        private readonly IMenuItemRepository _menuItemRepository;// I don't think I need to keep this 
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
                OrderItem newItem = new OrderItem(menuItem, DateTime.Now, Status.New, quantity);
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

        public void ClearSelectedItemsFromSession(ISession session)
        {
            session.Remove("SelectedItems");
        }

        //public void AddItemsToOrder(int orderId, List<OrderItem> items)
        //{
        //    foreach (OrderItem item in items)
        //    {
        //        item.OrderID = orderId;
        //        _orderItemRepository.Insert(item);
        //    }
        //}

        //public Order TakeNewOrder(int tableId, Employee employee)
        //{
        //    return _orderRepository.TakeNewOrder(tableId, employee);
        //}

        //public Order GetOrderById(int orderID)
        //{
        //    return _orderRepository.GetOrderByID(orderID);
        //}


        ////add item to an existing order
        //public void AddSingleItemToOrder(int orderID, int itemID, int quantity)
        //{
        //    Order order = _orderRepository.GetOrderByID(orderID);
        //    if (order == null)
        //    {
        //        throw new Exception($"Order with ID {orderID} not found.");
        //    }

        //    //calling an item by ID
        //    MenuItem item = _menuItemRepository.GetMenuItemByID(itemID);

        //    if (item == null)
        //    {
        //        throw new Exception("Menu item not found.");
        //    }


        //    OrderItem newItem = new OrderItem(
        //    itemID,
        //    item,
        //    DateTime.Now,
        //    Status.New,
        //    quantity);

        //    AddOrUpdateOrderItem(order, newItem);
        //}

        //public void AddOrUpdateOrderItem(Order order, OrderItem newItem)
        //{
        //    bool exists = _orderRepository.OrderItemExists(order.OrderID, newItem.ItemID);

        //    if (exists)
        //    {
        //        _orderRepository.UpdateOrderItem(order.OrderID, newItem);
        //    }
        //    else
        //    {
        //        _orderRepository.InsertOrderItem(order.OrderID, newItem);
        //    }
        //}

    }
}
