using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Order TakeNewOrder(int tableId, Employee employee);
        Order GetOrderByID(int orderID);

    }
}
