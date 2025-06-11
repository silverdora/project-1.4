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
        public IActionResult ViewOrder(int orderId)
        {
            var orderSummary = _dummyOrderService.GetOrderSummaryById(orderId);
            if (orderSummary == null) return NotFound();

            var paymentViewModel = new FinishOrderViewModel
            {
                OrderID = orderId,
                AmountPaid = orderSummary.TotalAmount,
                LowVatAmount = orderSummary.LowVAT,
                HighVatAmount = orderSummary.HighVAT
                // populate other fields as needed
            };

            return View(paymentViewModel);
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

                // Redirect to confirmation or success page
                return RedirectToAction("Confirmation", "Order");
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
        public IActionResult SplitBill(SplitPaymentViewModel model)
        {
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


 