using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class OrderDetailsViewModel
    {

        public int OrderID { get; set; }
        public string EmployeeName { get; set; }
        public int TableNumber { get; set; }
        public DateTime OrderTime { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }
}
