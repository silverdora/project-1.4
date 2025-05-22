using Chapeau.Models;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;
using Chapeau.Repositories;
using Chapeau.ViewModels;

namespace Chapeau.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly IMenuItemService _menuItemService;

        public MenuItemController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        public IActionResult Index(MenuCard? card, MenuCategory? category, int? orderID)
        {
            var filteredItems = _menuItemService.GetFilteredMenuItems(card, category);

            var viewModel = new MenuSelectionViewModel
            {
                SelectedCard = card,
                SelectedCategory = category,
                Items = filteredItems
            };

            // This fills the list with ALL items from the selected card
            if (card != null)
            {
                viewModel.AllItemsForSelectedCard = _menuItemService.GetMenuItemsByCard(card.Value);
            }

            ViewBag.CurrentOrderID = orderID;

            return View(viewModel);
        }
    }   
}