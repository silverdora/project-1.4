using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Repositories
{
    public class DummyOrderRepository : IDummyOrderRepository
    {
        private static List<Order> orders = new List<Order>();

        public IEnumerable<Order> GetAllOrders()
        {
            return orders;
        }

        public Order GetOrderById(int id)
        {
            return orders.FirstOrDefault(o => o.OrderID == id);
        }

        public void AddOrder(Order order)
        {
            orders.Add(order);
        }

        public void UpdateOrderStatus(int orderId, Status status)
        {
            var order = GetOrderById(orderId);
            if (order != null)
            {
                order.Status = status;
            }
        }
    }
}
