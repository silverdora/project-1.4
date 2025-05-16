namespace Chapeau.Models
{
    public class MenuItem
    {
        public int ItemID { get; set; }
        public string Item_name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal VATPercent { get; set; }
        public string Category { get; set; }
        public int StockQuantity { get; set; }

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

        public MenuItem(int itemID, string item_name, string description, decimal price, decimal vATPercent, string category, int stockQuantity)
        {
            ItemID = itemID;
            Item_name = item_name;
            Description = description;
            Price = price;
            VATPercent = vATPercent;
            Category = category;
            StockQuantity = stockQuantity;
        }
    }
}
