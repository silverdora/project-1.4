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

        public List<MenuItem> GetAllMenuItems() //ask repository to ge All items from database
        {
            return _menuItemRepository.GetAll();
        }
    }
}