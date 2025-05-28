using System;
using Chapeau.Models;

namespace Chapeau.ViewModels
{
	public class RunningOrdersViewModel
	{
		public List<Order> NewOrders;
        public List<Order> InProgessOrders;
        public List<Order> ReadyOrders;

        Dictionary<int, List<MenuCategory>> NewOrdersByCourse;

        Dictionary<int, List<MenuCategory>> PreparingOrdersByCourse;

        Dictionary<int, List<MenuCategory>> ReadyOrdersByCourse;

        public Employee Employee;

        public RunningOrdersViewModel(List<Order> newOrders, List<Order> inProgessOrders, List<Order> readyOrders, Dictionary<int, List<MenuCategory>> newOrdersByCourse, Dictionary<int, List<MenuCategory>> preparingOrdersByCourse, Dictionary<int, List<MenuCategory>> readyOrdersByCourse, Employee employee)
        {
            NewOrders = newOrders;
            InProgessOrders = inProgessOrders;
            ReadyOrders = readyOrders;
            NewOrdersByCourse = newOrdersByCourse;
            PreparingOrdersByCourse = preparingOrdersByCourse;
            ReadyOrdersByCourse = readyOrdersByCourse;
            Employee = employee;
        }
    }
}

