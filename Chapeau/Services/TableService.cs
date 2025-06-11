using Chapeau.Models;
using Chapeau.Repository;
using Chapeau.Repository.Interface;
using Chapeau.Service.Interface;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;


namespace Chapeau.Service
{
    public class TableService : ITableService
  
    {
        private readonly ITableRepository _tableRepository;


        public TableService(ITableRepository tableRepository)
        {
            _tableRepository = tableRepository;
        }

        public List<Table> GetAllTables() => _tableRepository.GetAllTables();
        
        public List<Table> GetTablesWithOrderStatus()
        {
            return _tableRepository.GetTablesWithOrderStatus();
        }
        public List<TableOrderViewModel> GetTableOverview()
        {
            return _tableRepository.GetTableOrderViewModels();
        }



        public void SetTableOccupiedStatus(int tableId, bool isOccupied)
        {
            _tableRepository.UpdateTableOccupiedStatus(tableId, isOccupied);
        }

        public bool TrySetTableFree(int tableId)
        {
            if (_tableRepository.HasUnservedOrders(tableId))
                return false;

            _tableRepository.UpdateTableOccupiedStatus(tableId, false);
            return true;
        }

        public void MarkOrderAsServed(int tableId)
        {
            _tableRepository.MarkReadyOrdersAsServed(tableId);
        }

        public void MarkTableFreeByOrder(int orderId)
        {
            _tableRepository.MarkTableFreeByOrder(orderId);
        }
    }
}



