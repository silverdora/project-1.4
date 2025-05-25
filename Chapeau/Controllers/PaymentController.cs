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

        public ActionResult Index()
        {
            var payments = _paymentService.GetAllPayments(1); // Use your real service method
            return View(payments);
        }
        public IActionResult ViewPaymentDetails(int orderID)
        {
            var order = _orderService.GetOrderById(orderID);

            if (order == null)
                return NotFound();

            decimal totalAmount = order.OrderItems.Sum(i => i.MenuItem.Price * i.Quantity);

            Payment payment = new Payment
            {
                orderID = order.OrderID,
                amountPaid = totalAmount,
                tipAmount = 0,
                paymentType = "Card",
                paymentDAte = DateTime.Now,
                lowVATAmount = 0, // just set to 0 for now
                highVATAmount = totalAmount * 0.21m // assume everything is high VAT
            };

            return View("Details", payment);
        }
    }
}

        // Show the complete order of a table to the waiter
