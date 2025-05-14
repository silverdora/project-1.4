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

        public List<MenuItem> GetByCardAndCategory(string card, string category)
        {
            return _menuItemRepository.GetByCardAndCategory(card, category);
        }

        public List<string> GetAllCards()
        {
            return _menuItemRepository.GetAllCards();
        }

        public List<string> GetAllCategories()
        {
            return _menuItemRepository.GetAllCategories();
        }


    }
}