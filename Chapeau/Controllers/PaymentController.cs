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
        public IActionResult CompletePayment(int orderID)
        {
            var order = _orderService.GetOrderById(orderID);

            if (order == null)
                return NotFound();

            Payment payment = new Payment
            {
                orderID = order.OrderID,
                amountPaid = order.TotalAmount,
                tipAmount = 0, // Add a form later if needed
                paymentType = "Card", // hardcoded for now
                paymentDAte = DateTime.Now,
                lowVATAmount = order.LowVATTotal,
                highVATAmount = order.HighVATTotal
            };

            _paymentService.AddPayment(payment);
            _orderService.MarkOrderAsCompleted(order.OrderID);

            return RedirectToAction("Index", "RunningOrders");
        }


    }
}
        // Show the complete order of a table to the waiter
       