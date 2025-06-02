using Microsoft.AspNetCore.Mvc;
using Chapeau.Services;
using Chapeau.Repositories;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
using Chapeau.Service;

namespace Chapeau.Controllers
{
    public class OrderController : Controller
    {
        private readonly DummyOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly TableService _tableService;

        public OrderController(
            DummyOrderService orderService,
            IPaymentService paymentService,
            TableService tableService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
            _tableService = tableService;
        }

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

        [HttpGet]
        public IActionResult FinishOrder(int orderId)
        {
            var viewModel = new FinishOrderViewModel { OrderID = orderId };
            return View("~/Views/DummyOrder/FinishOrder.cshtml", viewModel);
        }

        [HttpPost]
        public IActionResult FinishOrder(FinishOrderViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/DummyOrder/FinishOrder.cshtml", model);

            _paymentService.SavePayment(model);
            _orderService.MarkOrderAsPaid(model.OrderID);

            ViewBag.Message = "Order successfully finished!";
            return View("Confirmation");
        }

        public IActionResult Confirmation()
        {
            return View("~/Views/DummyOrder/Confirmation.cshtml");
        }
        [HttpGet]
        public IActionResult SplitBill(int orderId, int numberOfPeople = 2)
        {
            // Retrieve total order amount from your order service
            decimal totalAmount = _orderService.GetOrderTotal(orderId);

            // Initialize payments list based on numberOfPeople
            var payments = new List<IndividualPayment>();
            for (int i = 0; i < numberOfPeople; i++)
            {
                payments.Add(new IndividualPayment
                {
                    // Default equal split
                    AmountPaid = Math.Round(totalAmount / numberOfPeople, 2)
                });
            }

            var model = new SplitPaymentViewModel
            {
                OrderID = orderId,
                TotalAmount = totalAmount,
                NumberOfPeople = numberOfPeople,
                Payments = payments
            };

            return View("~/Views/DummyOrder/SplitBill.cshtml", model);
        }

        [HttpPost]
        public IActionResult SplitBill(SplitPaymentViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/DummyOrder/SplitBill.cshtml", model);

            // Optional: Validate sum of AmountPaid >= TotalAmount

            // Save each individual payment via your payment service
            foreach (var payment in model.Payments)
            {
                _paymentService.SaveIndividualPayment(model.OrderID, payment.AmountPaid, payment.TipAmount, payment.PaymentType, payment.Feedback);
            }

            _orderService.MarkOrderAsPaid(model.OrderID);

            return View("~/Views/DummyOrder/Confirmation.cshtml");
        }

    }

}