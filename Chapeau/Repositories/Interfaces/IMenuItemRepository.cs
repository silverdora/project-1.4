using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IMenuItemRepository
    {
        public List<MenuItem> GetMenuItems();
        public List<MenuItem> GetMenuItemsByCard(MenuCard card);
        public List<MenuItem> GetMenuItemsByCategory(MenuCategory category);
        List<MenuItem> GetMenuItemsByCardAndCategory(MenuCard card, MenuCategory category);

    }
}
