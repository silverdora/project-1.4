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
        public IActionResult FinishOrder(FinishOrderViewModel model, string action)
        {
            var order = _orderService.GetOrderSummaryById(model.OrderID);
            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Overview", "Restaurant");
            }

            // Remove any validation errors for feedback
            ModelState.Remove("Feedback");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validate that the amount paid matches the order total
            if (Math.Abs(model.AmountPaid - order.TotalAmount) > 0.01m) // Allow for small rounding differences
            {
                ModelState.AddModelError("", "The amount paid must match the order total.");
                return View(model);
            }

            // Save payment
            _paymentService.SavePayment(model);

            // Mark the order as paid
            _paymentService.MarkOrderAsPaid(model.OrderID);

            TempData["SuccessMessage"] = "Payment completed successfully!";
            return RedirectToAction("Overview", "Restaurant");
        }
        [HttpGet]
        public IActionResult SplitBill(int orderId)
        {
            var order = _orderService.GetOrderSummaryById(orderId);
            if (order == null) return RedirectToAction("Overview", "Restaurant");

            var model = new SplitPaymentViewModel
            {
                OrderID = orderId,
                TotalAmount = order.TotalAmount,
                NumberOfPeople = 2,
                Payments = new List<IndividualPayment>()
            };

            // Initialize with 2 people
            decimal amountPerPerson = Math.Round(order.TotalAmount / 2, 2);
            decimal remainingAmount = order.TotalAmount - amountPerPerson;

            model.Payments.Add(new IndividualPayment { AmountPaid = amountPerPerson, TipAmount = 0 });
            model.Payments.Add(new IndividualPayment { AmountPaid = remainingAmount, TipAmount = 0 });

            return View(model);
        }

        [HttpPost]
        public IActionResult SplitBill(SplitPaymentViewModel model, string action)
        {
            var order = _orderService.GetOrderSummaryById(model.OrderID);
            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Overview", "Restaurant");
            }

            // Handle adding a new person
            if (action == "add" && model.NumberOfPeople < 4)
            {
                model.NumberOfPeople++;
                decimal amountPerPerson = Math.Round(order.TotalAmount / model.NumberOfPeople, 2);
                decimal remainingAmount = order.TotalAmount - (amountPerPerson * (model.NumberOfPeople - 1));

                model.Payments = new List<IndividualPayment>();
                for (int i = 0; i < model.NumberOfPeople; i++)
                {
                    model.Payments.Add(new IndividualPayment
                    {
                        AmountPaid = i == model.NumberOfPeople - 1 ? remainingAmount : amountPerPerson,
                        TipAmount = 0,
                        PaymentType = PaymentType.Cash // Default payment type
                    });
                }
                return View(model);
            }

            // Remove any validation errors for feedback
            for (int i = 0; i < model.Payments.Count; i++)
            {
                ModelState.Remove($"Payments[{i}].Feedback");
            }

            // This is the final submission
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validate that the total amount paid matches the order total
            decimal totalPaid = model.Payments.Sum(p => p.AmountPaid);
            if (Math.Abs(totalPaid - order.TotalAmount) > 0.01m) // Allow for small rounding differences
            {
                ModelState.AddModelError("", "The total amount paid must match the order total.");
                return View(model);
            }

            // Save each individual payment
            foreach (var payment in model.Payments)
            {
                var paymentModel = new Payment
                {
                    orderID = model.OrderID,
                    amountPaid = payment.AmountPaid,
                    tipAmount = payment.TipAmount,
                    paymentType = payment.PaymentType,
                    paymentDAte = DateTime.Now,
                    Feedback = string.IsNullOrEmpty(payment.Feedback) ? null : payment.Feedback
                };

                _paymentService.AddPayment(paymentModel);
            }

            // Mark the order as paid
            _paymentService.MarkOrderAsPaid(model.OrderID);

            TempData["SuccessMessage"] = "Split payment completed successfully!";
            return RedirectToAction("Overview", "Restaurant");
        }
    }
}

