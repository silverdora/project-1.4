using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using System;

namespace Chapeau.Services
{
	public class RunningOrdersService:IRunningOrdersService
	{
        private readonly IRunningOrdersRepository _runningOrdersRepository;

        public RunningOrdersService(IRunningOrdersRepository runningOrdersRepository)
        {
            _runningOrdersRepository = runningOrdersRepository;
        }

        public void ChangeOrderStatus(int itemID, Status status)
        {
            _runningOrdersRepository.ChangeOrderStatus(itemID, status);
        }

        public List<Order> GetAllBarOrders()
        {
            return _runningOrdersRepository.GetAllBarOrders();
        }

        public List<Order> GetAllKitchenOrders()
        {
            return _runningOrdersRepository.GetAllKitchenOrders();
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
                            if (!categoriesByOrderId.ContainsKey(order.OrderID))
                            {
                                categoriesByOrderId.Add(order.OrderID, new List<MenuCategory> {course});
                            }
                            else
                            {
                                categoriesByOrderId[order.OrderID].Add(course);
                            }
                        }
                    }
                }
            }
            return categoriesByOrderId;
        }
    }
}

