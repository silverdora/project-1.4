using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Service.Interface
{
    public interface ITableService
    {
        List<Table> GetAllTables();
        List<Table> GetTablesWithOrderStatus();
        List<TableOrderViewModel> GetTableOverview();
        // sprint 3
        void SetTableOccupiedStatus(int tableId, bool isOccupied);
        bool TrySetTableFree(int tableId);
        void MarkOrderAsServed(int tableId);


    }
}
