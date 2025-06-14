using System;
using Chapeau.Models;

namespace Chapeau.ViewModels
{
	public class FinishedOrdersViewModel
    {
        public List<Order> ReadyOrders;
        public Dictionary<int, List<MenuCategory>> ReadyOrdersByCourse;
        public Employee Employee;

        public FinishedOrdersViewModel(List<Order> readyOrders, Dictionary<int, List<MenuCategory>> readyOrdersByCourse, Employee employee)
        {
            ReadyOrders = readyOrders;
            ReadyOrdersByCourse = readyOrdersByCourse;
            Employee = employee;
        }
    }
}

