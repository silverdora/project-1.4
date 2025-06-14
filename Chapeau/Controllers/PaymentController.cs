using Chapeau.Repositories.Interfaces;
using Chapeau.Repositories;
using Chapeau.Services;
using Chapeau.Services.Interfaces;
using Chapeau.Models;
using Chapeau.Services;

using Microsoft.AspNetCore.Mvc;
using Chapeau.ViewModels;

namespace Chapeau.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        // Use ONE constructor
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
           
        }

        public ActionResult Index()
        {
            var payments = _paymentService.GetAllPayments(1); // Use your real service method
            return View(payments);
        }

        //Mo added in order to free the table after payment is done Sprint3
        [HttpPost]
        [HttpGet]
        public IActionResult ProcessPayment(int orderId)
        {
            var model = new PaymentViewModel
            {
                OrderID = orderId
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult ProcessPayment(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _paymentService.MarkOrderAsPaid(model.OrderID);

            // Optional: If i want to free the table automatically after payment,
            //_tableService.TrySetTableFree(tableId); // Requires getting tableId from Order

            TempData["SuccessMessage"] = $"Payment for Order #{model.OrderID} completed successfully.";
            return RedirectToAction("Index", "Restaurant");
        }

        [HttpPost]
        public IActionResult StartPayment(int tableId)
        {
            int? orderId = _paymentService.GetLatestUnpaidOrderIdByTable(tableId);
            if (orderId == null)
            {
                TempData["Error"] = "No unpaid order found for this table.";
                return RedirectToAction("Index", "Restaurant");
            }

            return RedirectToAction("ProcessPayment", new { orderId = orderId.Value });
        }


    }
}

 