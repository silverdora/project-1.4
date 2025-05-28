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
           
            try
            {
                // ⬇ Use the Sprint 2 method to include order status
                var tables = _tableService.GetTablesWithOrderStatus();
                return View(tables);
            }
            catch (Exception ex)
            {
                return Content($"ERROR: {ex.Message} \n\n {ex.StackTrace}");
            }

        }
    }
}
