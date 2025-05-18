using Chapeau.Models;
//using static Chapeau.HelperMethods.MenuItemFilters;

namespace Chapeau.Services
{
    public interface IMenuItemService
    {
        public List<MenuItem> GetAllMenuItems();
        public List<MenuItem> GetMenuItemsByCard(MenuCard card);
        public List<MenuItem> GetMenuItemsByCategory(MenuCategory category);

        List<MenuItem> GetMenuItemsByCardAndCategory(MenuCard card, MenuCategory category);


    }
}
