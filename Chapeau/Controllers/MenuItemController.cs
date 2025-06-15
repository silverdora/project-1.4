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
            List<MenuItem> filteredItems = _menuItemService.GetFilteredMenuItems(card, category);

            MenuSelectionViewModel viewModel = new MenuSelectionViewModel
            {
                SelectedCard = card,
                SelectedCategory = category,
                Items = filteredItems,
                OrderID = orderID ?? 0,
                TableID = tableID ?? 0
            };

            return View(viewModel);
        }
    }
}
