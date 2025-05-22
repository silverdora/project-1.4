using System;
using Chapeau.Models;

namespace Chapeau.Services
{
	public interface IRunningOrdersService
	{
        List<Order> GetAllBarOrders();
        List<Order> GetAllKitchenOrders();
        List<Order> GetBarOrdersByStatus(Status status);
        List<Order> GetKitchenOrdersByStatus(Status status);
        void ChangeOrderStatus(int itemID, Status status);

        Order GetCompleteOrderForTable(int tableId);
        void CloseOrder(int orderId);


    }
}


