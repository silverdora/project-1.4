﻿namespace Chapeau.Models
{
    public class MenuItem
    {
        public int ItemId { get; set; }
        public string Item_name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal VATPercent { get; set; }
        public MenuCategory Category { get; set; }
        public int StockQuantity { get; set; }
        public string Card { get; set; }
        public bool IsOutOfStock
        {
            get
            {
                return StockQuantity == 0;
            }
        }

        public bool IsLowStock
        {
            get
            {
                return StockQuantity > 0 && StockQuantity <= 10;
            }
        }
        

    }
}
