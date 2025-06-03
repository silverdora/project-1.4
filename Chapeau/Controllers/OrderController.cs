using Chapeau.Models;
using Chapeau.Services;
using Chapeau.HelperMethods;
using Microsoft.AspNetCore.Mvc;
using Chapeau.Models.Extensions;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
using Chapeau.Service;

namespace Chapeau.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly DummyOrderService _dummyOrderService;
        private readonly IMenuItemService _menuItemService;
        private readonly IPaymentService _paymentService;
        private readonly TableService _tableService;
       


        public OrderController(
            IOrderService orderService,
            DummyOrderService dummyOrderService,
            IPaymentService paymentService,
            TableService tableService,
            IMenuItemService menuItemService
            )
        {
            _orderService = orderService;
            _dummyOrderService = dummyOrderService;
            _paymentService = paymentService;
            _tableService = tableService;
            _menuItemService = menuItemService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult TakeOrder(int tableId)
        {
            Employee? employee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");

            if (employee == null)
                return RedirectToAction("Login", "Employee");

            Order? existingOrder = _orderService.GetActiveOrderByTableId(tableId);

            Order order;

            if (existingOrder != null)
            {
                order = existingOrder;
            }
            else
            {
                order = new Order
                {
                    Table = new Table { TableId = tableId },
                    Employee = employee,
                    OrderTime = DateTime.Now,
                    IsPaid = false,
                   
                };

                _orderService.InsertOrder(order);
            }

            HttpContext.Session.SetInt32("CurrentOrderId", order.OrderID);
            HttpContext.Session.SetInt32("CurrentTableId", tableId);

            // Clear any old items
            HttpContext.Session.Remove("SelectedItems");

            return RedirectToAction("Index", "MenuItem");
        }           
        
        [HttpPost]
        public IActionResult AddItem(int menuItemId, int quantity)
        {
            int? orderId = HttpContext.Session.GetInt32("CurrentOrderId");
            int? tableId = HttpContext.Session.GetInt32("CurrentTableId");

            if (orderId == null || tableId == null)
            {
                TempData["Error"] = "No active order found. Please start a new order.";
                return RedirectToAction("Index", "MenuItem");
            }

            _orderService.AddItemToSessionSelection(menuItemId, quantity, HttpContext.Session);

            MenuItem item = _menuItemService.GetMenuItemByID(menuItemId);
            TempData["AddedMessage"] = $"{quantity} × \"{item.Item_name}\" added successfully!";

            return RedirectToAction("Index", "MenuItem");
        }
        [HttpGet]
        public IActionResult OrderDetails()
        {
            //reusing helper method to read the session value for the key "SelectedItems"
            List<OrderItem> selectedItems = HttpContext.Session.GetSelectedItemsFromSession();

            return View(selectedItems);
        }
       
        [HttpPost]
        public IActionResult SubmitOrder()
        {
            int? orderId = HttpContext.Session.GetInt32("CurrentOrderId");

            if (orderId == null)
            {
                TempData["OrderError"] = "No active order found.";
                return RedirectToAction("Overview", "Restaurant");
            }

            List<OrderItem> selectedItems = HttpContext.Session.GetSelectedItemsFromSession();

            if (selectedItems != null && selectedItems.Count > 0)
            {
                _orderService.AddItemsToOrder(orderId.Value, selectedItems);

                foreach (var item in selectedItems)
                {
                    _menuItemService.ReduceStock(item.MenuItem.ItemID, item.Quantity);
                }
                // clearing order/table data to be able to start a new one 
                HttpContext.Session.Remove("SelectedItems");
                HttpContext.Session.Remove("CurrentOrderId");
                HttpContext.Session.Remove("CurrentTableId");

                TempData["OrderSuccess"] = "The order was submitted successfully!";
            }

            return RedirectToAction("Overview", "Restaurant");
        }

        [HttpGet]
        public IActionResult ViewOrder(int tableId)
        {
            var summary = _dummyOrderService.GetOrderSummary(tableId);
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
            _dummyOrderService.MarkOrderAsPaid(model.OrderID);

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
            decimal totalAmount = _dummyOrderService.GetOrderTotal(orderId);

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

            _dummyOrderService.MarkOrderAsPaid(model.OrderID);

            return View("~/Views/DummyOrder/Confirmation.cshtml");
        }

    }

}