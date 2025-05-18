using System;
using Chapeau.Models;

namespace Chapeau.ViewModels
{
	public class RunningOrdersViewModel
	{
		public List<Order> NewOrders;
        public List<Order> InProgessOrders;
        public List<Order> ReadyOrders;
        //public Employee Employee;
        public RunningOrdersViewModel()
		{
		}

        public RunningOrdersViewModel(List<Order> newOrders, List<Order> inProgessOrders, List<Order> readyOrders)
        {
            NewOrders = newOrders;
            InProgessOrders = inProgessOrders;
            ReadyOrders = readyOrders;
        }
    }
}

