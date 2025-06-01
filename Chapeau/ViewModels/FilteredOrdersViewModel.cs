using System;
using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class FilteredOrdersViewModel
    {
        public List<Order> Orders;
        public Status Status;
        public FilteredOrdersViewModel()
        {
        }

        public FilteredOrdersViewModel(List<Order> orders, Status status)
        {
            Orders = orders;
            Status = status;
        }
    }
}

