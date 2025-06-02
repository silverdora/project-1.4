using Chapeau.Models;
using System.Collections.Generic;


namespace Chapeau.Repository.Interface
{
    public interface ITableRepository
    {
        List<Table> GetAllTables();
        List<Table> GetTablesWithOrderStatus();

        void MarkTableFreeByOrder(int orderId);

    }
}
