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
        private readonly IOrderService _orderService;
        // Use ONE constructor
        public PaymentController(IPaymentService paymentService, IOrderService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            try
            {
                var payments = _paymentService.GetAllPayments(1);
                return View(payments);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to retrieve payments: " + ex.Message;
                return RedirectToAction("Overview", "Restaurant");
            }
        }

        //Mo added in order to free the table after payment is done Sprint3
        [HttpPost]
        [HttpGet]
        public IActionResult ProcessPayment(int orderId)
        {
            try
            {
                var model = new PaymentViewModel { OrderID = orderId };
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to process payment: " + ex.Message;
                return RedirectToAction("Overview", "Restaurant");
            }
        }

        [HttpPost]
        public IActionResult ProcessPayment(PaymentViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                _paymentService.MarkOrderAsPaid(model.OrderID);
                TempData["SuccessMessage"] = $"Payment for Order #{model.OrderID} completed successfully.";
                return RedirectToAction("Overview", "Restaurant");
            }
            catch (Exception ex)
            {
                TempData["SuccessMessage"] = $"Payment for Order #{model.OrderID} completed successfully.";
                return RedirectToAction("ViewOrder", "Payment", new { orderId = model.OrderID });
            }
        }

        [HttpPost]
        public IActionResult StartPayment(int tableId)
        {
            try
            {
                int? orderId = _paymentService.GetLatestUnpaidOrderIdByTable(tableId);
                if (orderId == null)
                {
                    TempData["Error"] = "No unpaid order found for this table.";
                    return RedirectToAction("Overview", "Restaurant");
                }

                return RedirectToAction("ProcessPayment", new { orderId = orderId.Value });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to start payment: " + ex.Message;
                return RedirectToAction("Overview", "Restaurant");
            }
        }

        public IActionResult ViewOrder(int tableId)
        {
            int? orderId = null;
            try
            {
                orderId = _paymentService.GetLatestUnpaidOrderIdByTable(tableId);
                if (orderId == null)
                {
                    TempData["Error"] = "No unpaid order found for this table.";
                    return RedirectToAction("Overview", "Restaurant");
                }

                var orderSummary = _orderService.GetOrderSummaryById(orderId.Value);
                if (orderSummary == null)
                {
                    TempData["Error"] = "Order summary not found.";
                    return RedirectToAction("Overview", "Restaurant");
                }
                return View(orderSummary);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to view order: {ex.Message} (orderId: {orderId})";
                return RedirectToAction("Overview", "Restaurant");
            }
        }

        [HttpGet]
        public IActionResult FinishOrder(int orderId)
        {
            try
            {
                var order = _orderService.GetOrderSummaryById(orderId);
                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction("Overview", "Restaurant");
                }

                var model = new FinishOrderViewModel
                {
                    OrderID = orderId,
                    LowVatAmount = order.LowVAT,
                    HighVatAmount = order.HighVAT,
                    AmountPaid = order.TotalAmount
                };
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to finish order: " + ex.Message;
                return RedirectToAction("Overview", "Restaurant");
            }
        }

        [HttpPost]
        public IActionResult FinishOrder(FinishOrderViewModel model, string action)
        {
            try
            {
                var order = _orderService.GetOrderSummaryById(model.OrderID);
                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction("Overview", "Restaurant");
                }

                ModelState.Remove("Feedback");

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (Math.Abs(model.AmountPaid - order.TotalAmount) > 0.01m)
                {
                    ModelState.AddModelError("", "The amount paid must match the order total.");
                    return View(model);
                }

                _paymentService.SavePayment(model);
                _paymentService.MarkOrderAsPaid(model.OrderID);

                TempData["SuccessMessage"] = "Payment completed successfully!";
                return RedirectToAction("Overview", "Restaurant");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to finish order: " + ex.Message;
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult SplitBill(int orderId)
        {
            try
            {
                var order = _orderService.GetOrderSummaryById(orderId);
                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction("Overview", "Restaurant");
                }

                var model = CreateSplitPaymentModel(order);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to split bill: " + ex.Message;
                return RedirectToAction("Overview", "Restaurant");
            }
        }

        [HttpPost]
        public IActionResult SplitBill(SplitPaymentViewModel model, string action)
        {
            try
            {
                var order = _orderService.GetOrderSummaryById(model.OrderID);
                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction("Overview", "Restaurant");
                }

                if (action == "add" && model.NumberOfPeople < 4)
                {
                    return HandleAddPerson(model, order);
                }

                if (!ValidateSplitPayment(model, order))
                {
                    return View(model);
                }

                ProcessSplitPayments(model);
                _paymentService.MarkOrderAsPaid(model.OrderID);

                TempData["SuccessMessage"] = "Split payment completed successfully!";
                return RedirectToAction("Overview", "Restaurant");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to process split bill: " + ex.Message;
                return View(model);
            }
        }

        private SplitPaymentViewModel CreateSplitPaymentModel(OrderSummaryViewModel order)
        {
            var model = new SplitPaymentViewModel
            {
                OrderID = order.OrderID,
                TotalAmount = order.TotalAmount,
                NumberOfPeople = 2, // Default to 2 people
                Payments = new List<IndividualPayment>()
            };

            decimal total = order.TotalAmount;
            int people = model.NumberOfPeople;
            decimal baseAmount = Math.Floor((total / people) * 100) / 100;
            decimal totalAssigned = baseAmount * people;
            int remainderCents = (int)Math.Round((total - totalAssigned) * 100);

            for (int i = 0; i < people; i++)
            {
                decimal amount = baseAmount;
                if (i < remainderCents)
                    amount += 0.01m;

                model.Payments.Add(new IndividualPayment { AmountPaid = amount, TipAmount = 0, PaymentType = PaymentType.Cash });
            }

            return model;
        }

        private IActionResult HandleAddPerson(SplitPaymentViewModel model, OrderSummaryViewModel order)
        {
            if (model.NumberOfPeople >= 4)
            {
                ModelState.AddModelError("NumberOfPeople", "Maximum number of people is 4");
                return View(model);
            }

            model.NumberOfPeople++;
            model.TotalAmount = order.TotalAmount;

            decimal total = model.TotalAmount;
            int people = model.NumberOfPeople;
            decimal baseAmount = Math.Floor((total / people) * 100) / 100;
            decimal totalAssigned = baseAmount * people;
            int remainderCents = (int)Math.Round((total - totalAssigned) * 100);

            model.Payments = new List<IndividualPayment>();
            for (int i = 0; i < people; i++)
            {
                decimal amount = baseAmount;
                if (i < remainderCents)
                    amount += 0.01m;

                model.Payments.Add(new IndividualPayment
                {
                    AmountPaid = amount,
                    TipAmount = 0,
                    PaymentType = PaymentType.Cash
                });
            }
            return View(model);
        }

        private bool ValidateSplitPayment(SplitPaymentViewModel model, OrderSummaryViewModel order)
        {
            if (model.NumberOfPeople < 2 || model.NumberOfPeople > 4)
            {
                ModelState.AddModelError("NumberOfPeople", "Number of people must be between 2 and 4");
                return false;
            }

            for (int i = 0; i < model.Payments.Count; i++)
            {
                ModelState.Remove($"Payments[{i}].Feedback");
            }

            if (!ModelState.IsValid)
            {
                return false;
            }

            decimal totalPaid = model.Payments.Sum(p => p.AmountPaid);
            if (Math.Abs(totalPaid - order.TotalAmount) > 0.01m)
            {
                ModelState.AddModelError("", "The total amount paid must match the order total.");
                return false;
            }

            return true;
        }

        private void ProcessSplitPayments(SplitPaymentViewModel model)
        {
            foreach (var payment in model.Payments)
            {
                _paymentService.SaveIndividualPayment(
                    model.OrderID,
                    payment.AmountPaid,
                    payment.TipAmount,
                    payment.PaymentType.ToString(),
                    payment.Feedback
                );
            }
        }
    }
}

