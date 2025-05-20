using Chapeau.Repositories.Interfaces;
using Chapeau.Repositories;
using Chapeau.Services;
using Chapeau.Services.Interfaces;
using Chapeau.Models;
using Chapeau.Services;

using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IRunningOrdersService _orderService; //// Make sure this is injected

        // ✅ Use ONE constructor
        public PaymentController(IPaymentService paymentService, IRunningOrdersService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
        }
        [HttpPost]
        public IActionResult Pay(int orderId, string paymentType, decimal tip)
        {
            var order = _orderService.GetOrderWithItems(orderId); // you'll add this next
            if (order == null)
                return NotFound();

            _paymentService.ProcessPayment(order, paymentType, tip);
            return RedirectToAction("Index", "Orders");
        }
        public ActionResult Index()
        {
            var payments = _paymentService.GetAllPayments(1); // Use your real service method
            return View(payments);
        }
    }
}


