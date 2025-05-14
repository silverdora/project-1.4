using Chapeau.Models;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;

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
            var cards = _menuItemService.GetAllCards();
            var categories = _menuItemService.GetAllCategories();

            var items = _menuItemService.GetByCardAndCategory(selectedCard, selectedCategory);

            ViewBag.AllCards = cards;
            ViewBag.AllCategories = categories;
            ViewBag.SelectedCard = selectedCard;
            ViewBag.SelectedCategory = selectedCategory;

            return View(items);
        }
    }
}