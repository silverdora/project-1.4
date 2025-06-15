using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;
using System;

namespace Chapeau.Services
{
    public class RunningOrdersService : IRunningOrdersService
    {
        private readonly IRunningOrdersRepository _runningOrdersRepository;
        private readonly IPaymentService _paymentService;

        public RunningOrdersService(IRunningOrdersRepository runningOrdersRepository, IPaymentService paymentService)
        {
            _runningOrdersRepository = runningOrdersRepository;
            _paymentService = paymentService;
        }

        public void ChangeOrderStatus(int orderID, int itemID, Status status)
        {
            _runningOrdersRepository.ChangeOrderStatus(orderID, itemID, status);
        }

        public void ChangeAllOrderItemsStatus(int orderID, Status currentStatus, Status newStatus)
        {
            _runningOrdersRepository.ChangeAllOrderItemsStatus(orderID, currentStatus, newStatus);
        }

        public List<Order> GetBarOrdersByStatus(Status status)
        {
            return _runningOrdersRepository.GetBarOrdersByStatus(status);
        }

        public List<Order> GetKitchenOrdersByStatus(Status status)
        {
            return _runningOrdersRepository.GetKitchenOrdersByStatus(status);
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
                            if (!categoriesByOrderId.ContainsKey(order.OrderId))
                            {
                                categoriesByOrderId.Add(order.OrderId, new List<MenuCategory> {course});
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
            _runningOrdersRepository.ChangeOrderItemsFromOneCourseStatus(orderID, currentStatus, newStatus, course);
        }
    }
}





