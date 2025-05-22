using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IMenuItemService
    {
        public List<MenuItem> GetMenuItems();
        public List<MenuItem> GetMenuItemsByCard(MenuCard card);
        public List<MenuItem> GetMenuItemsByCategory(MenuCategory category);

        List<MenuItem> GetMenuItemsByCardAndCategory(MenuCard card, MenuCategory category);
        List<MenuItem> GetFilteredMenuItems(MenuCard? card, MenuCategory? category);

    }
}
