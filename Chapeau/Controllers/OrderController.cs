using Chapeau.Models;
using Chapeau.Services;
using Chapeau.HelperMethods;
using Microsoft.AspNetCore.Mvc;
using Chapeau.Models.Extensions;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
using Chapeau.Service;
using Chapeau.Exceptions;

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

        [HttpPost]
        public IActionResult TakeOrder(int tableId)
        {
            Employee? employee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");

            if (employee == null)
                return RedirectToAction("Login", "Employee");

            Order newOrder = new Order
            {
                Table = new Chapeau.Models.Table { TableId = tableId },//to avoid ambiguous reference with name Table (I was having an error)
                Employee = employee,
                OrderTime = DateTime.Now,
            };

            _orderService.InsertOrder(newOrder);

            //Store in session
            HttpContext.Session.SetInt32("CurrentOrderId", newOrder.OrderID);
            HttpContext.Session.SetInt32("CurrentTableId", tableId);

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

            List<OrderItem> selectedItems = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("SelectedItems");
            
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

            List<OrderItem> selectedItems = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("SelectedItems");

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

        //bar or kitchen methods

        [HttpGet]
        public IActionResult Index()
        {
            //get Employee object 
            Employee? loggedInEmployee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");
            if (loggedInEmployee == null)
            {
                throw new Exception("no user");
            }

            if ((loggedInEmployee.Role == Role.Bar) || (loggedInEmployee.Role == Role.Kitchen))
            {
                string type = (loggedInEmployee.Role == Role.Bar) ? "Drink" : "Dish";
                List<Order> newOrders = _orderService.GetOrdersByStatus(Status.Ordered, type);
                Dictionary<int, List<MenuCategory>> newOrdersByCourse = _orderService.GetCategoriesOfAnOrder(newOrders);
                List<Order> preparingOrders = _orderService.GetOrdersByStatus(Status.InProgress, type);
                Dictionary<int, List<MenuCategory>> preparingOrdersByCourse = _orderService.GetCategoriesOfAnOrder(preparingOrders);
                RunningOrdersViewModel runningOrdersViewModel = new RunningOrdersViewModel(newOrders, preparingOrders, newOrdersByCourse, preparingOrdersByCourse, loggedInEmployee);
                return View(runningOrdersViewModel);
            }

            else
            {
                throw new Exception("no access");
            }
        }

        [HttpGet]
        public IActionResult FinishedOrders()
        {
            Employee? loggedInEmployee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");

            if (loggedInEmployee == null)
            {
                throw new Exception("no user");
            }

            if ((loggedInEmployee.Role == Role.Bar) || (loggedInEmployee.Role == Role.Kitchen))
            {
                string type = (loggedInEmployee.Role == Role.Bar) ? "Drink" : "Dish";
                List<Order> readyOrders = _orderService.GetOrdersByStatus(Status.ReadyToBeServed, type);
                List<Order> servedOrders = _orderService.GetOrdersByStatus(Status.Served, type);
                Dictionary<int, List<MenuCategory>> readyOrdersByCourse = _orderService.GetCategoriesOfAnOrder(readyOrders);
                Dictionary<int, List<MenuCategory>> servedOrdersByCourse = _orderService.GetCategoriesOfAnOrder(servedOrders);
                ReadyToBeServedOrdersViewModel toBeServedOrdersViewModel = new ReadyToBeServedOrdersViewModel(readyOrders, servedOrders, readyOrdersByCourse, servedOrdersByCourse, loggedInEmployee);
                return View(toBeServedOrdersViewModel);
            }

            else
            {
                throw new Exception("no access");
            }
        }

        [HttpPost]
        public IActionResult ChangeOrderItemStatus(int orderItemID, Status status)
        {
            try
            {
                _orderService.ChangeOrderItemStatus(orderItemID, status);
                TempData["StatusChangeMessage"] = "Status has been changed.";
            }
            catch (ChangeStatusException)
            {
                TempData["StatusChangeError"] = "Status change failed.";
            }
            
            if (status == Status.Served)
            {
                return RedirectToAction("FinishedOrders");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ChangeAllOrderItemsStatus(int orderID, Status currentStatus, Status newStatus)
        {
            Employee? loggedInEmployee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");
            try
            {
                if (loggedInEmployee.Role == Role.Kitchen)
                {
                    _orderService.ChangeAllOrderItemsStatus(orderID, "Dish", currentStatus, newStatus);
                }
                else
                {
                    _orderService.ChangeAllOrderItemsStatus(orderID, "Drink", currentStatus, newStatus);
                }

                TempData["StatusChangeMessage"] = "Status has been changed.";
            }
            catch (ChangeStatusException)
            {
                TempData["StatusChangeError"] = "Status change failed.";
            }

            if (newStatus == Status.Served)
            {
                return RedirectToAction("FinishedOrders");
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult ChangeOrderItemsFromOneCourseStatus(int orderID, Status currentStatus, Status newStatus, MenuCategory course)
        {
            try
            {
                _orderService.ChangeOrderItemsFromOneCourseStatus(orderID, currentStatus, newStatus, course);
                TempData["StatusChangeMessage"] = "Status has been changed.";
            }
            catch (ChangeStatusException)
            {
                TempData["StatusChangeError"] = "Status change failed.";
            }
            if (newStatus == Status.Served)
            {
                return RedirectToAction("FinishedOrders");
            }
            return RedirectToAction("Index");
        }

        //payment methods

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