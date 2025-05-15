namespace Chapeau.Models
{
    public class OrdemItem
    {   public int OrderID { get; set; }
        public int ItemID { get; set; }// create an obj of menu item instead of int

        public DateTime IncludeDate { get; set; }
        public string Status { get; set; }
        public int Quantity { get; set; }
    }
}
