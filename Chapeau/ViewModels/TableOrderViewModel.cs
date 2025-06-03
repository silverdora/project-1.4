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

        public bool HasFoodOrder => FoodStatus.HasValue;
        public bool HasDrinkOrder => DrinkStatus.HasValue;

        //public bool CanMarkAsServed => FoodStatus == Status.Ready || DrinkStatus == Status.Ready;
        //public bool CanFreeTable => IsOccupied && !HasFoodOrder && !HasDrinkOrder;
    }
}
