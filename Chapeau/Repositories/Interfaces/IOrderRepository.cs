using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        //taking an order
        void InsertOrder(Order order);

        //bar or kitchen
        List<Order> GetOrdersByStatus(Status status, string type, DateTime createdAfter);
        

        Order? GetActiveOrderByTableId(int tableId);

        Order GetOrderById(int orderId);

        void MarkOrderAsPaid(int orderId);


    }
}
