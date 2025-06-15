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
            TempData["Success"] = $"Table {tableId} marked as occupied.";
            return RedirectToAction("Overview");
        }

        [HttpPost]
        public IActionResult SetFree(int tableId)
        {
            bool success = _tableService.TrySetTableFree(tableId);
            TempData[success ? "Success" : "Error"] = success
                ? $"Table {tableId} is now free."
                : $"Cannot free Table {tableId}: active orders exist.";
            return RedirectToAction("Overview");
        }


        [HttpPost]
        public IActionResult MarkOrderServed(int tableId)
        {
            _tableService.MarkOrderAsServed(tableId);
            TempData["Success"] = $"Order at Table {tableId} ready orders were marked as served..";
            return RedirectToAction("Overview");

        }

    }
}
