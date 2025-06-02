using Chapeau.ViewModels;
using Chapeau.Repositories;

using Microsoft.Data.SqlClient;


namespace Chapeau.Services
    
{
    public class DummyOrderService
    {
        private readonly DummyOrderRepository _repo;

        private readonly string _connectionString;

        public DummyOrderService(IConfiguration configuration)
        {
            _repo = new DummyOrderRepository(configuration);
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Simple method to get order summary by table ID
        public OrderSummaryViewModel GetOrderSummary(int tableId)
        {
            var order = _repo.GetActiveOrderByTable(tableId);
            if (order == null)
                return null;

            // Group order items by item name
            var groupedItems = order.OrderItems
                .GroupBy(i => i.MenuItem.Item_name)
                .Select(g => new OrderItemViewModel
                {
                    ItemName = g.Key,
                    Quantity = g.Sum(i => i.Quantity),
                    UnitPrice = g.First().MenuItem.Price,
                    VATRate = g.First().MenuItem.VATPercent
                }).ToList();

            // Calculate totals
            decimal totalAmount = groupedItems.Sum(i => i.Quantity * i.UnitPrice);
            decimal lowVAT = groupedItems
                .Where(i => i.VATRate == 9)
                .Sum(i => i.Quantity * i.UnitPrice * 0.09m);
            decimal highVAT = groupedItems
                .Where(i => i.VATRate == 21)
                .Sum(i => i.Quantity * i.UnitPrice * 0.21m);

            return new OrderSummaryViewModel
            {
                OrderID = order.OrderID,
                TableNumber = tableId,
                Items = groupedItems,
                TotalAmount = totalAmount,
                LowVAT = lowVAT,
                HighVAT = highVAT
            };
        }

        // Mark the order as paid in the database
        public void MarkOrderAsPaid(int orderId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "UPDATE [Order] SET isPaid = 1 WHERE orderID = @orderId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}