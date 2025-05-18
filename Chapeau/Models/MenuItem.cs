namespace project-1.4.Models
{
    public class MenuItem
    {
        public int itemID { get; set; }
        public string item_name { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public decimal VATPercent { get; set; }
        public string category { get; set; }
        public int stockQuantity { get; set; }
        public int cardID { get; set; }
        public string item_type { get; set; }
        public bool IsActive { get; set; }
    }
}