using Chapeau.Models;

namespace Chapeau.Service.Interface
{
    public interface ITableService
    {
        List<Table> GetAllTables();

        void MarkTableFreeByOrder(int orderId);

    }
}
