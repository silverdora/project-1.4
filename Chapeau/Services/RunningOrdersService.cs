using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;
using System;

namespace Chapeau.Services
{
    public class RunningOrdersService : IRunningOrdersService
    {
        private readonly IRunningOrdersRepository _runningOrdersRepository;

        public RunningOrdersService(IRunningOrdersRepository runningOrdersRepository)
        {
            _runningOrdersRepository = runningOrdersRepository;
        }

        public void ChangeOrderStatus(int orderItemID, Status status)
        {
            _runningOrdersRepository.ChangeOrderItemStatus(orderItemID, status);
        }

        public void ChangeAllOrderItemsStatus(int orderID, string type, Status currentStatus, Status newStatus)
        {
            // ...foreach orderitem in orderitems in ordercard
            _runningOrdersRepository.ChangeAllOrderItemsStatus(orderID, type, currentStatus, newStatus);
        }

        public List<Order> GetOrdersByStatus(Status status, string type)
        {
            DateTime createdAfter = DateTime.Today;
            return _runningOrdersRepository.GetOrdersByStatus(status, type, createdAfter);
        }

        
        public Dictionary<int, List<MenuCategory>> GetCategoriesOfAnOrder(List<Order> orders)
        {
            Dictionary<int, List<MenuCategory>> categoriesByOrderId = new Dictionary<int, List<MenuCategory>>();

            List<MenuCategory> courses = new List<MenuCategory> { MenuCategory.Starters, MenuCategory.Mains, MenuCategory.Desserts, MenuCategory.Entremets, MenuCategory.Beer, MenuCategory.Wine, MenuCategory.Spirits, MenuCategory.Coffee, MenuCategory.Tea, MenuCategory.SoftDrink };
            foreach (MenuCategory course in courses)
            {
                foreach(Order order in orders)
                {
                    foreach (OrderItem orderItem in order.OrderItems)
                    {
                        if (orderItem.MenuItem.Category == course)
                        {
                            if (!categoriesByOrderId.ContainsKey(order.OrderID))
                            {
                                categoriesByOrderId.Add(order.OrderID, new List<MenuCategory> {course});
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
            _runningOrdersRepository.ChangeOrderItemsFromOneCourseStatus(orderID, currentStatus, newStatus, course);
        }
    }
}





