using System;
using Chapeau.Models;

namespace Chapeau.ViewModels
{
	public class RunningOrdersViewModel
	{
		public List<Order> Orders;
		//public Employee Employee;
		public RunningOrdersViewModel()
		{
		}

        public RunningOrdersViewModel(List<Order> orders)
        {
            Orders = orders;
            //Employee = employee;
        }
    }
}

