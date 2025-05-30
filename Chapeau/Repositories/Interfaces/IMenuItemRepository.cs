using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IMenuItemRepository
    {
        List<MenuItem> GetMenuItems();
        MenuItem GetMenuItemByID(int itemID);
        List<MenuItem> GetMenuItemsByCard(string cardName);
        List<MenuItem> GetMenuItemsByCategory(string categoryName);
        List<MenuItem> GetMenuItemsByCardAndCategory(string cardName, string categoryName);

    }
}
