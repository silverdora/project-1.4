using System;
using Chapeau.Models;

namespace Chapeau.ViewModels
{
	public class ReadyToBeServedOrdersViewModel
	{
        public List<Order> ReadyToBeServedOrders;
        public List<Order> ServedOrders;

        public Dictionary<int, List<MenuCategory>> ReadyToBeServedOrdersByCourse;
        public Dictionary<int, List<MenuCategory>> ServedOrdersByCourse;
        public Employee Employee;

        public ReadyToBeServedOrdersViewModel(List<Order> readyOrders, List<Order> servedOrders, Dictionary<int, List<MenuCategory>> readyOrdersByCourse, Dictionary<int, List<MenuCategory>> servedOrdersByCourse, Employee employee)
        {
            ReadyToBeServedOrders = readyOrders;
            ServedOrders = servedOrders;
            ReadyToBeServedOrdersByCourse = readyOrdersByCourse;
            ServedOrdersByCourse = servedOrdersByCourse;
            Employee = employee;
        }
    }
}


