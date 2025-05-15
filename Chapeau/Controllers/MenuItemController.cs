using Chapeau.Models;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;
using Chapeau.HelperMethods;    

namespace Chapeau.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly IMenuItemService _menuItemService;

        public MenuItemController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        public IActionResult Index(string selectedCard = "All", string selectedCategory = "All")
        {
            ViewBag.Cards = MenuItemFilters.Cards;
            ViewBag.Categories = MenuItemFilters.Categories;

            ViewBag.SelectedCard = selectedCard;
            ViewBag.SelectedCategory = selectedCategory;

            var items = _menuItemService.GetByCardAndCategory(selectedCard, selectedCategory);
            return View(items);
        }


    }
}