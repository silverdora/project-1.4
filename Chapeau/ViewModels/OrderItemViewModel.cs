using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class OrderItemViewModel
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VATRate { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;

        // Mo Add these two for status display in table overview
        public Status Status { get; set; }
        public string ItemType { get; set; } // "Drink" or "Dish"

    }
}
