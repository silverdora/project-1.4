using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IOrderRepository
    {    
        public void InsertOrder(Order order);
    }
}
