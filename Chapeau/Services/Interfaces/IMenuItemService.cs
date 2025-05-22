using Chapeau.Models;
using Chapeau;

namespace Chapeau.Services
{
    public interface IMenuItemService
    {
        List<MenuItem> GetAllMenuItems();
        List<MenuItem> GetMenuItemsByCard(MenuCard card);
        List<MenuItem> GetMenuItemsByCategory(MenuCategory category);
        List<MenuItem> GetMenuItemsByCardAndCategory(MenuCard card, MenuCategory category);
        MenuItem GetMenuItemById(int id);
        void AddMenuItem(MenuItem item);
        void UpdateMenuItem(MenuItem item);
        void ToggleMenuItemActive(int id, bool isActive);
    }
}