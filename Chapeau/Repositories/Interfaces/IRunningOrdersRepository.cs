using System;
using Chapeau.Models;
namespace Chapeau.Repositories.Interfaces
{
    public interface IRunningOrdersRepository
    {
        List<Order> GetBarOrdersByStatus(Status status);
        List<Order> GetKitchenOrdersByStatus(Status status);
<<<<<<< HEAD
        void ChangeOrderStatus(int itemID, Status status);

     

=======
        void ChangeOrderStatus(int orderID, int itemID, Status status);
        void ChangeAllOrderItemsStatus(int orderID, Status currentStatus, Status newStatus);
>>>>>>> dev
    }
}

