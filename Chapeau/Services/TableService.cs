using Chapeau.Models;
using Chapeau.Repository;
using Chapeau.Repository.Interface;
using Chapeau.Service.Interface;
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

        public void MarkTableFreeByOrder(int orderId)
        {
            _tableRepository.MarkTableFreeByOrder(orderId);
        }
    }
}



