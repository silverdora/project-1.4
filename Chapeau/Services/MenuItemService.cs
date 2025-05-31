using Chapeau.Models;
using Chapeau.Repositories.Interfaces;

namespace Chapeau.Services
{
    public class MenuItemService:IMenuItemService
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public MenuItemService(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }

        public List<MenuItem> GetMenuItems()
        {
            return _menuItemRepository.GetMenuItems();
        }

        public List<MenuItem> GetFilteredMenuItems(string? card, string? category)
        {
            if (!string.IsNullOrWhiteSpace(card) && !string.IsNullOrWhiteSpace(category))
            {
                return _menuItemRepository.GetMenuItemsByCardAndCategory(card, category);
            }
            else if (!string.IsNullOrWhiteSpace(card))
            {
                return _menuItemRepository.GetMenuItemsByCard(card);
            }
            else if (!string.IsNullOrWhiteSpace(category))
            {
                return _menuItemRepository.GetMenuItemsByCategory(category);
            }
            else
            {
                return _menuItemRepository.GetMenuItems();
            }
        }
        public MenuItem GetMenuItemByID(int itemID)
        {
            return _menuItemRepository.GetMenuItemByID(itemID);
        }

        public void ReduceStock(int itemId, int amount)
        {
            _menuItemRepository.ReduceStock(itemId, amount);
        }
    }

}
