using Microsoft.AspNetCore.Mvc;
using Chapeau.Models;
using Chapeau.Service.Interface;
using Chapeau.Models.Extensions;


namespace Chapeau.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly ITableService _tableService;

        public RestaurantController(ITableService tableService)
        {
            _tableService = tableService;
        }

        public IActionResult Overview()
        {
            var tables = _tableService.GetAllTables();
            return View(tables);
        }
    }
}

