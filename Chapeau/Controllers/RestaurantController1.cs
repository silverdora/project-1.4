using Microsoft.AspNetCore.Mvc;
using Chapeau.Models;
using Chapeau.Service.Interface;
using Chapeau.Models.Extensions;


namespace Chapeau.Controllers
{
    public class RestaurantController1 : Controller
    {
        private readonly ITableService _tableService;

        public RestaurantController1(ITableService tableService)
        {
            _tableService = tableService;
        }

        public IActionResult Overview()
        {
            try
            {
                var tables = _tableService.GetAllTables();
                return View(tables);
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not implemented here)
                return StatusCode(500, "An error occurred while retrieving the tables.");
            }
        }
    }
}

