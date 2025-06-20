﻿using Chapeau.Models;

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

        //lulu
        public int? OrderID { get; set; } // Nullable if no active order


       

        //to show the items on the Table 
        public List<OrderItemViewModel> FoodItems { get; set; } = new();
        public List<OrderItemViewModel> DrinkItems { get; set; } = new();

        public bool HasReadyToBeServedItems =>
        FoodItems.Any(i => i.Status == Status.ReadyToBeServed) ||
        DrinkItems.Any(i => i.Status == Status.ReadyToBeServed);

    }
}
