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

        public List<MenuItem> GetMenuItemsByCard(MenuCard card)
        {
            return _menuItemRepository.GetMenuItemsByCard(card);
        }

        public List<MenuItem> GetMenuItemsByCategory(MenuCategory category)
        {
            return _menuItemRepository.GetMenuItemsByCategory(category);
        }

        public List<MenuItem> GetMenuItemsByCardAndCategory(MenuCard card, MenuCategory category)
        {
            return _menuItemRepository.GetMenuItemsByCardAndCategory(card, category);
        }
        public List<MenuItem> GetFilteredMenuItems(MenuCard? card, MenuCategory? category)
        {
            if (card != null && category != null)
            {
                return GetMenuItemsByCardAndCategory(card.Value, category.Value);
            }
            else if (card != null)
            {
                return GetMenuItemsByCard(card.Value);
            }
            else if (category != null)
            {
                return GetMenuItemsByCategory(category.Value);
            }
            else
            {
                return GetMenuItems(); // all items
            }
        }

    }
}