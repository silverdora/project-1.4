namespace Chapeau.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int EmployeeID { get; set; }
        public int TableID { get; set; }
        public DateTime OrderTime { get; set; }
        public bool IsServed { get; set; }

        public List<Includes> Includes { get; set; } = new List<Includes>();
    }
}
