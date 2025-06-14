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
        private readonly DummyOrderService _orderService;
        // Use ONE constructor
        public PaymentController(IPaymentService paymentService, DummyOrderService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
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

        public IActionResult ViewOrder(int tableId)
        {
            int? orderId = _paymentService.GetLatestUnpaidOrderIdByTable(tableId);
            if (orderId == null)
            {
                TempData["Error"] = "No unpaid order found for this table.";
                return RedirectToAction("Index", "Restaurant");
            }

            var orderSummary = _orderService.GetOrderSummaryById(orderId.Value);
            if (orderSummary == null)
            {
                TempData["Error"] = "Order summary not found.";
                return RedirectToAction("Index", "Restaurant");
            }
            return View(orderSummary);
        }

        [HttpGet]
        public IActionResult FinishOrder(int orderId)
        {
            // You can fetch order/payment details here if needed
            var model = new FinishOrderViewModel { OrderID = orderId };
            return View(model);
        }

        [HttpPost]
        public IActionResult FinishOrder(FinishOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Save payment, mark order as paid, etc.
            _paymentService.SavePayment(model);
            TempData["SuccessMessage"] = "Payment completed successfully!";
            return RedirectToAction("Index", "Restaurant");
        }

    }
}

 