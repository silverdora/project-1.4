using Chapeau.Models;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;
using Chapeau.Repositories;
using System.Linq;

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

            if (card != null && category != null)
            {
                items = _menuItemService.GetMenuItemsByCardAndCategory(card.Value, category.Value);
            }
            else if (card != null)
            {
                items = _menuItemService.GetMenuItemsByCard(card.Value);
            }
            else if (category != null)
            {
                items = _menuItemService.GetMenuItemsByCategory(category.Value);
            }
            else
            {
                items = _menuItemService.GetAllMenuItems();
            }

            ViewBag.SelectedCard = card;
            ViewBag.SelectedCategory = category;
            ViewBag.Cards = Enum.GetValues(typeof(MenuCard));
            ViewBag.Categories = Enum.GetValues(typeof(MenuCategory));

            return View(items);
        }


        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Cards = Enum.GetValues(typeof(MenuCard));
            ViewBag.Categories = Enum.GetValues(typeof(MenuCategory));
            return View();
        }

        [HttpPost]
        public IActionResult Add(MenuItem item)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("Add Errors: " + string.Join(" | ", errors));
            }

            if (ModelState.IsValid)
            {
                _menuItemService.AddMenuItem(item);
                return RedirectToAction("Index");
            }

            ViewBag.Cards = Enum.GetValues(typeof(MenuCard));
            ViewBag.Categories = Enum.GetValues(typeof(MenuCategory));
            return View(item);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _menuItemService.GetMenuItemById(id);
            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Cards = Enum.GetValues(typeof(MenuCard));
            ViewBag.Categories = Enum.GetValues(typeof(MenuCategory));
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(MenuItem item)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("Edit Errors: " + string.Join(" | ", errors));
            }

            if (ModelState.IsValid)
            {
                _menuItemService.UpdateMenuItem(item);
                return RedirectToAction("Index");
            }

            ViewBag.Cards = Enum.GetValues(typeof(MenuCard));
            ViewBag.Categories = Enum.GetValues(typeof(MenuCategory));
            return View(item);
        }

       
        [HttpPost]
        public IActionResult ToggleActive(int id, bool isActive)
        {
            _menuItemService.ToggleMenuItemActive(id, isActive);
            return RedirectToAction("Index");
        }
    }
}
