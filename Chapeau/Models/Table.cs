using System;
namespace Chapeau.Models
{
	public class Table
	{
		public int TableId { get; set; }
        public int TableNumber { get; set; }
		public bool IsOccupied { get; set; }
        public Table()
		{
		}

        public Table(int tableId, int tableNumber, bool isOccupied)
        {
            TableId = tableId;
            TableNumber = tableNumber;
            IsOccupied = isOccupied;
        }
    }
}

