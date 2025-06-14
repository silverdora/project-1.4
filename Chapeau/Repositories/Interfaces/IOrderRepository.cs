using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IOrderRepository
    {    
        void InsertOrder(Order order);

        Order? GetActiveOrderByTableId(int tableId);


   

    }
}
