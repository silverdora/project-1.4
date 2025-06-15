using Chapeau.ViewModels;
using Chapeau.ViewModels;
using Chapeau.Repositories;

using Microsoft.Data.SqlClient;
using Chapeau.Models;


namespace Chapeau.Services

{
    public class DummyOrderService
    {
        private readonly PaymentRepository _paymentRepository;

        private readonly string _connectionString;

        public DummyOrderService(IConfiguration configuration)
        {
            _paymentRepository = new PaymentRepository(configuration);
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Simple method to get order summary by table ID
        public OrderSummaryViewModel GetOrderSummaryById(int orderId)
        {
            var order = _paymentRepository.GetOrderById(orderId);
            if (order == null)
                return null;

            var groupedItems = order.OrderItems
                .GroupBy(i => i.MenuItem.Item_name)
                .Select(g => new OrderItemViewModel
                {
                    ItemName = g.Key,
                    Quantity = g.Sum(i => i.Quantity),
                    UnitPrice = g.First().MenuItem.Price,
                    VATRate = g.First().MenuItem.VATPercent
                }).ToList();

            decimal totalAmount = groupedItems.Sum(i => i.Quantity * i.UnitPrice);
            decimal lowVAT = groupedItems
                .Where(i => i.VATRate == 9)
                .Sum(i => i.Quantity * i.UnitPrice * 0.09m);
            decimal highVAT = groupedItems
                .Where(i => i.VATRate == 21)
                .Sum(i => i.Quantity * i.UnitPrice * 0.21m);

            return new OrderSummaryViewModel
            {
                OrderID = order.OrderId,
                TableNumber = order.Table.TableId,  // Set the table number
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
        public Order GetOrderById(int orderId)
        {
            return _paymentRepository.GetOrderById(orderId);
        }

        public decimal GetOrderTotal(int orderId)
        {
            var order = GetOrderById(orderId);
            if (order == null || order.OrderItems == null || order.OrderItems.Count == 0)
                return 0;

            return order.OrderItems.Sum(item => item.MenuItem.Price * item.Quantity);
        }



    }
}