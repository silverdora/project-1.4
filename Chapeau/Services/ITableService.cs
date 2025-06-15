using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Service.Interface
{
    /// <summary>
    /// Interface for table-related operations in the restaurant system.
    /// </summary>
    public interface ITableService
    {
        /// <summary>
        /// Retrieves all tables from the database.
        /// </summary>
        /// <returns>List of all tables.</returns>
        List<Table> GetAllTables();

        /// <summary>
        /// Retrieves all tables with their current order status.
        /// </summary>
        /// <returns>List of tables with order status.</returns>
        List<Table> GetTablesWithOrderStatus();

        /// <summary>
        /// Gets an overview of all tables, including their orders and statuses.
        /// </summary>
        /// <returns>List of table order view models.</returns>
        List<TableOrderViewModel> GetTableOverview();
        // sprint 3
        /// <summary>
        /// Sets the occupied status of a table.
        /// </summary>
        /// <param name="tableId">The ID of the table.</param>
        /// <param name="isOccupied">True to mark as occupied, false to mark as free.</param>
        void SetTableOccupiedStatus(int tableId, bool isOccupied);

        /// <summary>
        /// Attempts to set a table as free if there are no active orders.
        /// </summary>
        /// <param name="tableId">The ID of the table.</param>
        /// <returns>True if the table was set free, false otherwise.</returns>
        bool TrySetTableFree(int tableId);

        /// <summary>
        /// Marks all ready orders for a table as served.
        /// </summary>
        /// <param name="tableId">The ID of the table.</param>
        void MarkOrderAsServed(int tableId);


        public Table? GetTableById(int tableId);//(matheus)

        /// <summary>
        /// Marks the table as free based on the order ID.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        void MarkTableFreeByOrder(int orderId);
    }
}
