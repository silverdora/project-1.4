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
        // GET: Show payment form for the order
        [HttpGet]
        public IActionResult PayOrder(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);  // Add this method if you don't have it
            if (order == null)
                return NotFound();

            var payment = new Payment
            {
                orderID = orderId,
                paymentDAte = DateTime.Now
            };
            return View(payment);
        }

        // POST: Process payment
        [HttpPost]
        public IActionResult PayOrder(Payment payment)
        {
            if (!ModelState.IsValid)
                return View(payment);

            // Store payment in DB
            _paymentService.AddPayment(payment);

            // Close order (mark as served/closed)
            _orderService.CloseOrder(payment.orderID);

            // Redirect back to orders page or payment index
            return RedirectToAction("Index", "RunningOrders");
        }
    }
}
    



