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

        public List<MenuItem> GetFilteredMenuItems(MenuCard? card, MenuCategory? category)
        {
            if (card != null && category != null)
            {
                return _menuItemRepository.GetMenuItemsByCardAndCategory(card.Value, category.Value);
            }
            else if (card != null)
            {
                return _menuItemRepository.GetMenuItemsByCard(card.Value);
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
