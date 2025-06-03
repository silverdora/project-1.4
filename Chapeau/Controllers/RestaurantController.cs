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
                //var tables = _tableService.GetTablesWithOrderStatus();
                // NEW Sprint 3 logic (returns List<TableOrderViewModel>)
                //var tables = _tableService.GetTableOverview();

                //return View(tables);
                var viewModels = _tableService.GetTableOverview();
                return View(viewModels);
            }
            catch (Exception ex)
            {
                return Content($"ERROR: {ex.Message} \n\n {ex.StackTrace}");
            }


        }
        [HttpPost]
        public IActionResult SetOccupied(int tableId)
        {
            _tableService.SetTableOccupiedStatus(tableId, true);
            return RedirectToAction("Overview");
        }

        [HttpPost]
        public IActionResult SetFree(int tableId)
        {
            bool success = _tableService.TrySetTableFree(tableId);
            if (!success)
                TempData["Error"] = "❌ Table cannot be freed because there are still active orders.";
            else
                TempData["Success"] = "✅ Table marked as free.";

            return RedirectToAction("Overview");
        }


        [HttpPost]
        public IActionResult MarkOrderServed(int tableId)
        {
            _tableService.MarkOrderAsServed(tableId);
            return RedirectToAction("Overview");

        }

    }
}
