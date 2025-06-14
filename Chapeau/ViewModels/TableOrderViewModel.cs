using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class TableOrderViewModel
    {
        public int TableId { get; set; }
        public int TableNumber { get; set; }
        public bool IsOccupied { get; set; }

        public Status? FoodStatus { get; set; }
        public Status? DrinkStatus { get; set; }
        //public int? OrderId { get; set; } // ✅ Small and safe///Mo added sprint3 to process payment

        public bool HasFoodOrder => FoodStatus.HasValue;
        public bool HasDrinkOrder => DrinkStatus.HasValue;

        //public bool CanMarkAsServed => FoodStatus == Status.Ready || DrinkStatus == Status.Ready;
        //public bool CanFreeTable => IsOccupied && !HasFoodOrder && !HasDrinkOrder;

        //to show the items on the Table 
        public List<OrderItemViewModel> FoodItems { get; set; } = new();
        public List<OrderItemViewModel> DrinkItems { get; set; } = new();

    }
}
