using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IOrderService
    {
        Order TakeNewOrder(int tableId, Employee employee);
        Order GetOrderById(int orderID);

    }
}
