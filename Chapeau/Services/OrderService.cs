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
            // You could set the table as occupied here if needed
            _orderRepository.InsertOrder(order);
        }

        public Order? GetActiveOrderByTableId(int tableId)
        {
            return _orderRepository.GetActiveOrderByTableId(tableId);
        }

        public void AddItemsToOrder(int orderId, List<OrderItem> items)
        {
            foreach (OrderItem item in items)
            {
                item.OrderId = orderId;
                _orderItemRepository.Insert(item);
            }
        }

    }
}