using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IMenuItemRepository
    {
        List<MenuItem> GetMenuItems();
        MenuItem GetMenuItemByID(int itemID);
        List<MenuItem> GetMenuItemsByCard(MenuCard card);
        List<MenuItem> GetMenuItemsByCategory(MenuCategory category);
        List<MenuItem> GetMenuItemsByCardAndCategory(MenuCard card, MenuCategory category);
        void ReduceStock(int itemId, int amount);

    }
}
