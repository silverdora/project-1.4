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
        private readonly DummyOrderService _dummyOrderService;

        public PaymentController(IPaymentService paymentService, DummyOrderService dummyOrderService)
        {
            _paymentService = paymentService;
            _dummyOrderService = dummyOrderService;
        }


        public ActionResult Index()
        {
            var payments = _paymentService.GetAllPayments(1); // Use your real service method
            return View(payments);
        }
        [HttpGet]
        public IActionResult ViewOrder(int id)
        {
            var orderSummary = _dummyOrderService.GetOrderSummaryById(id);
            if (orderSummary == null) return NotFound();

            return View(orderSummary);
        }

        [HttpGet]
        public IActionResult FinishOrder(int orderId)
        {
            var viewModel = new FinishOrderViewModel { OrderID = orderId };
            return View("FinishOrder", viewModel);
        }

        [HttpPost]
        public IActionResult FinishOrder(FinishOrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Call your payment service to save payment details
                _paymentService.SavePayment(model);

                // Mark order as paid, maybe via DummyOrderService or payment service
                _dummyOrderService.MarkOrderAsPaid(model.OrderID);

                // Redirect to confirmation or success page in PaymentController
                return RedirectToAction("Confirmation", "Payment");
            }

            // If validation fails, return view with model to show errors
            return View(model);
        }


        [HttpGet]
        public IActionResult SplitBill(int orderId, int numberOfPeople = 2)
        {
            decimal totalAmount = _dummyOrderService.GetOrderTotal(orderId);

            var payments = Enumerable.Range(0, numberOfPeople)
                .Select(_ => new IndividualPayment { AmountPaid = Math.Round(totalAmount / numberOfPeople, 2) })
                .ToList();

            var model = new SplitPaymentViewModel
            {
                OrderID = orderId,
                TotalAmount = totalAmount,
                NumberOfPeople = numberOfPeople,
                Payments = payments
            };

            return View("SplitBill", model);
        }

        [HttpPost]
        public IActionResult SplitBill(SplitPaymentViewModel model, string action)
        {
            if (action == "update")
            {
                // Recalculate split
                decimal totalAmount = _dummyOrderService.GetOrderTotal(model.OrderID);
                var payments = Enumerable.Range(0, model.NumberOfPeople)
                    .Select(_ => new IndividualPayment { AmountPaid = Math.Round(totalAmount / model.NumberOfPeople, 2) })
                    .ToList();

                model.TotalAmount = totalAmount;
                model.Payments = payments;
                return View("SplitBill", model);
            }

            // Confirm payment
            if (!ModelState.IsValid)
                return View("SplitBill", model);

            foreach (var payment in model.Payments)
            {
                _paymentService.SaveIndividualPayment(model.OrderID, payment.AmountPaid, payment.TipAmount, payment.PaymentType, payment.Feedback);
            }

            _dummyOrderService.MarkOrderAsPaid(model.OrderID);

            return View("Confirmation");
        }
        public IActionResult Confirmation()
        {
            return View("Confirmation");
        }

     
    }

}


 