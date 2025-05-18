using Chapeau.Models;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;
using Chapeau.Repositories;    

namespace Chapeau.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly IMenuItemService _menuItemService;

        public MenuItemController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        public IActionResult Index(MenuCard? card, MenuCategory? category)
        {
            List<MenuItem> items = new List<MenuItem>();

            // Both filters selected
            if (card != null && category != null)
            {
                items = _menuItemService.GetMenuItemsByCardAndCategory(card.Value, category.Value);
            }
            // card selected
            else if (card != null)
            {
                items = _menuItemService.GetMenuItemsByCard(card.Value);
            }
            //  category selected
            else if (category != null)
            {
                items = _menuItemService.GetMenuItemsByCategory(category.Value);
            }
            //No filters
            else
            {
                items = _menuItemService.GetAllMenuItems();
            }

            // Send enum values and selected filters to the view
            ViewBag.SelectedCard = card;
            ViewBag.SelectedCategory = category;
            ViewBag.Cards = Enum.GetValues(typeof(MenuCard));
            ViewBag.Categories = Enum.GetValues(typeof(MenuCategory));

            return View(items);
        }
    }

   
}