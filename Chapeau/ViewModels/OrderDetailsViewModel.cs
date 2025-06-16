using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class OrderDetailsViewModel
    {
        public int OrderID { get; set; }
        public int TableNumber { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

    }
}
