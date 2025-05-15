namespace Chapeau.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int EmployeeID { get; set; }// have to create an object 
        public int TableID { get; set; }// have to create an object 
        public DateTime OrderTime { get; set; }
        public bool IsServed { get; set; }

        public List<Includes> Includes { get; set; } = new List<Includes>();
    }
}
