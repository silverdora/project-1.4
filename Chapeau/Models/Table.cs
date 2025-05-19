namespace Chapeau.Models
{
    public class Table
    {
        public int TableID { get; set; }
        public TableStatus Status { get; set; }
        public int TableNumber { get; set; }

        public Table()
        {

        }
        public Table(int tableNumber)
        {
            TableNumber = tableNumber;
        }
    }
}
