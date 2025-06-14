using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        //taking an order
        public void InsertOrder(Order order);

        //bar or kitchen
        List<Order> GetOrdersByStatus(Status status, string type, DateTime createdAfter);
        
    }
}
