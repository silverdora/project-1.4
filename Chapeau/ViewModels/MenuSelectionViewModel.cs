using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class MenuSelectionViewModel
    {
        //public MenuCard? SelectedCard { get; set; }
        //public MenuCategory? SelectedCategory { get; set; }
        public string? SelectedCard { get; set; }
        public string? SelectedCategory { get; set; }
        public List<MenuItem> Items { get; set; }

        //// Holds user selection, using int= itemID and  int quantity
        //public Dictionary<int, int> Quantities { get; set; } = new();

        public string Notes { get; set; } = string.Empty;

        //to be able to show the buttons that are not current 
        public List<MenuItem> AllItemsForSelectedCard { get; set; } = new();
        public int OrderID { get; set; } // ✅ NEW

        public int TableID { get; set; }


    }
}
