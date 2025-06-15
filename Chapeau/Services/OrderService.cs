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
                            if (!categoriesByOrderId.ContainsKey(order.OrderId))
                            {
                                categoriesByOrderId.Add(order.OrderId, new List<MenuCategory> { course });
                            }
                            else
                            {
                                if (!categoriesByOrderId[order.OrderId].Contains(course))
                                {
                                    categoriesByOrderId[order.OrderId].Add(course);
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