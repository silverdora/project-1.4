using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using static Chapeau.HelperMethods.MenuItemFilters;

namespace Chapeau.Services
{
    public class MenuItemService:IMenuItemService
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public MenuItemService(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }
      
        public List<MenuItem> GetAllMenuItems()
        {
            return _menuItemRepository.GetAllMenuItems();
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

    }
}