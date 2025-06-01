using Chapeau.Models;
using Chapeau.Repository;
using Chapeau.Repository.Interface;
using Chapeau.Service.Interface;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Chapeau.Service
{
    public class TableService : ITableService
    {

        private ITableRepository _tableRepo;

        public TableService(ITableRepository tableRepo)
        {
            _tableRepo = tableRepo;
        }

        public List<Table> GetAllTables() => _tableRepo.GetAllTables();
        public List<Table> GetTablesWithOrderStatus()
        {
            return _tableRepo.GetTablesWithOrderStatus();
        }
        public List<TableOrderViewModel> GetTableOverview()
        {
            //return _tableRepo.GetTableOverview();
            return _tableRepo.GetTableOrderViewModels();

        }


        public void SetTableOccupiedStatus(int tableId, bool isOccupied)
        {
            _tableRepo.UpdateTableOccupiedStatus(tableId, isOccupied);
        }

        public bool TrySetTableFree(int tableId)
        {
            if (_tableRepo.HasUnservedOrders(tableId))
                return false;

            _tableRepo.UpdateTableOccupiedStatus(tableId, false);
            return true;
        }

        public void MarkOrderAsServed(int tableId)
        {
            _tableRepo.MarkReadyOrdersAsServed(tableId);
        }


    }
}




