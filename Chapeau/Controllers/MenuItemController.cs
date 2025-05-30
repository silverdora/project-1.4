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

        public IActionResult Index(string? card, string? category, int? orderID, int? tableID)
        {
            var filteredItems = _menuItemService.GetFilteredMenuItems(card, category);

            var viewModel = new MenuSelectionViewModel
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
