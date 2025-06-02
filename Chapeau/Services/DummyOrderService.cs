using Chapeau.ViewModels;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class DummyOrderService
    {
        private readonly DummyOrderRepository _repo;

        public DummyOrderService(IConfiguration configuration)
        {
            _repo = new DummyOrderRepository(configuration);
        }

        public OrderSummaryViewModel GetOrderSummary(int tableId)
        {
            var order = _repo.GetActiveOrderByTable(tableId);
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

            decimal total = groupedItems.Sum(i => i.TotalPrice);
            decimal lowVAT = groupedItems.Where(i => i.VATRate == 9).Sum(i => i.TotalPrice * 0.09m);
            decimal highVAT = groupedItems.Where(i => i.VATRate == 21).Sum(i => i.TotalPrice * 0.21m);

            return new OrderSummaryViewModel
            {
                TableNumber = tableId,
                Items = groupedItems,
                TotalAmount = total,
                LowVAT = lowVAT,
                HighVAT = highVAT
            };
        }
    }
}
