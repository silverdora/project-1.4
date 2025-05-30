//using Chapeau.Models;

namespace Chapeau.Models.ViewModels
{
    public class TableOrderViewModel
    {
        public int TableId { get; set; }
        public int TableNumber { get; set; }
        public bool IsOccupied { get; set; }

        public Status? OrderStatus { get; set; }

        public bool CanMarkAsServed => OrderStatus == Status.ReadyToBeServed;
        public bool CanFreeTable => IsOccupied && OrderStatus == null;
    }

}
