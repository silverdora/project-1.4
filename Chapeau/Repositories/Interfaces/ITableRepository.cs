using Chapeau.Models;
using Chapeau.ViewModels;
using System.Collections.Generic;


namespace Chapeau.Repository.Interface
{
    public interface ITableRepository
    {
        List<Table> GetAllTables();
        List<Table> GetTablesWithOrderStatus();
        List<TableOrderViewModel> GetTableOverview();
        //List<TableOrderViewModel> GetTableOrderViewModels();
        // sprint 3
        void UpdateTableOccupiedStatus(int tableId, bool isOccupied);
        bool HasUnservedOrders(int tableId);
        void MarkReadyOrdersAsServed(int tableId);

        public Table? GetTableById(int tableId); //(matheus)
        void MarkTableFreeByOrder(int orderId);

        //Mo.....Sprint3
        void MarkOrderAsPaid(int orderId);
        int? GetLatestUnpaidOrderIdByTable(int tableId);


    }
}
