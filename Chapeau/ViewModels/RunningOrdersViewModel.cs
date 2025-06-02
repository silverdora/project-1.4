using System;
using Chapeau.Models;

namespace Chapeau.ViewModels
{
	public class RunningOrdersViewModel
	{
		public List<Order> NewOrders;
        public List<Order> InProgessOrders;

        public Dictionary<int, List<MenuCategory>> NewOrdersByCourse;
        public Dictionary<int, List<MenuCategory>> PreparingOrdersByCourse;

        public Employee Employee;

        public RunningOrdersViewModel(List<Order> newOrders, List<Order> inProgessOrders, Dictionary<int, List<MenuCategory>> newOrdersByCourse, Dictionary<int, List<MenuCategory>> preparingOrdersByCourse, Employee employee)
        {
            NewOrders = newOrders;
            InProgessOrders = inProgessOrders;
            NewOrdersByCourse = newOrdersByCourse;
            PreparingOrdersByCourse = preparingOrdersByCourse;
            Employee = employee;
        }
    }
}

