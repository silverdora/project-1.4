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

        public IActionResult Index(MenuCard? card, MenuCategory? category, int? orderID, int? tableID)
        {
            var filteredItems = _menuItemService.GetFilteredMenuItems(card, category);

            var viewModel = new MenuSelectionViewModel
            {
                SelectedCard = card,
                SelectedCategory = category,
                Items = filteredItems,
                OrderID = orderID ?? 0,   // 0 = no order yet
                TableID = tableID ?? 0    // ✅ capture the selected table
            };

            if (card != null)
            {
                viewModel.AllItemsForSelectedCard = _menuItemService.GetMenuItemsByCard(card.Value);
            }

            return View(viewModel);
        }
    }
}
