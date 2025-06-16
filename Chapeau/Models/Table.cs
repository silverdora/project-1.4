namespace Chapeau.Models
{
    public class Table
    {
        public int TableId { get; set; }
        public int TableNumber { get; set; }
        public bool IsOccupied { get; set; }
        public Status? OrderStatus { get; set; }// Mo for sprint 2


        public Table()
        {
        }

        public Table(int tableId, int tableNumber, bool isOccupied, Status? orderStatus)
        {
            TableId = tableId;
            TableNumber = tableNumber;
            IsOccupied = isOccupied;
            OrderStatus = orderStatus;
        }

    }
}