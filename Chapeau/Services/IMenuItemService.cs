using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IMenuItemService
    {
        public List<MenuItem> GetMenuItems();
        List<MenuItem> GetFilteredMenuItems(MenuCard? card, MenuCategory? category);
        MenuItem GetMenuItemByID(int itemID);
        void ReduceStock(int itemId, int amount);

    }
}
