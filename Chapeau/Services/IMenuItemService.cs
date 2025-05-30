using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IMenuItemService
    {
        public List<MenuItem> GetMenuItems();
       // public List<MenuItem> GetMenuItemsByCard(string? card);
        //public List<MenuItem> GetMenuItemsByCategory(string? category);

        //List<MenuItem> GetMenuItemsByCardAndCategory(string? card, string? category);
        List<MenuItem> GetFilteredMenuItems(string? card, string? category);

    }
}
