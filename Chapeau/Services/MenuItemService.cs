using Chapeau.Models;
using Chapeau.Repositories.Interfaces;

namespace Chapeau.Services
{
    public class MenuItemService : IMenuItemService
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

        
        public void AddMenuItem(MenuItem item)
        {
            _menuItemRepository.AddMenuItem(item);
        }

        
        public void UpdateMenuItem(MenuItem item)
        {
            _menuItemRepository.UpdateMenuItem(item);
        }

        public MenuItem GetMenuItemById(int id)
        {
            return _menuItemRepository.GetMenuItemById(id);
        }
        public void ToggleMenuItemActive(int id, bool isActive)
        {
            _menuItemRepository.ToggleMenuItemActive(id, isActive);
        }
    }
}
