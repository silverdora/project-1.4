using System;
using Chapeau.Models;

namespace Chapeau.ViewModels
{
	public class RunningOrdersViewModel
	{
		public Order Order;
		public List<OrderItem> OrderItems;
		public Status Status;
		public RunningOrdersViewModel()
		{
		}

        public RunningOrdersViewModel(Order order, Status status)
        {
            this.Order = order;
			this.Status = status;
            this.OrderItems = order.OrderItems;
        }
    }
}

