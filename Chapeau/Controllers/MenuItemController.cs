using Chapeau.Models;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly IMenuItemService _menuService;

        public MenuItemController(IMenuItemService menuService)
        {
            _menuService = menuService;
        }

        public IActionResult Index()
        {
            List<MenuItem> menuItems = _menuService.GetAllMenuItems();
            return View(menuItems);
        }
    }
}