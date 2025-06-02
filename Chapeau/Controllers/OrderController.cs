using Microsoft.AspNetCore.Mvc;
using Chapeau.Services;
using Chapeau.Repositories;

namespace Chapeau.Controllers
{
    public class OrderController : Controller
    {
        private readonly DummyOrderService _orderService;

        public OrderController(DummyOrderService orderService)
        {
            _orderService = orderService;
        }
        private readonly DummyOrderRepository _orderRepo;
        public IActionResult Index()
        {
            return View();
        }
      

        [HttpGet]
        public IActionResult ViewOrder(int tableId)
        {
            var summary = _orderService.GetOrderSummary(tableId);
            if (summary == null)
                return NotFound("No active order for this table.");

            return View("~/Views/DummyOrder/ViewOrder.cshtml", summary);

        }

    }
}
