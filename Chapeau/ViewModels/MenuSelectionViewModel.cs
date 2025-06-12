using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class MenuSelectionViewModel
    {
        public int TableID { get; set; }
        public int OrderID { get; set; } 
      
        public string? SelectedCard { get; set; }
        public string? SelectedCategory { get; set; }
        public List<MenuItem> Items { get; set; } = new List<MenuItem>();
    }
}
