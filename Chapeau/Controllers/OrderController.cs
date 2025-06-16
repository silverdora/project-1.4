using Chapeau.Models;
using Chapeau.Services;
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
        private readonly IMenuItemService _menuItemService;
        private readonly TableService _tableService;

        public OrderController(IOrderService orderService,TableService tableService,IMenuItemService menuItemService)
        {
            _orderService = orderService;
            _tableService = tableService;
            _menuItemService = menuItemService;
        }
        [HttpPost]
        public IActionResult TakeOrder(int tableId)
        {
            Employee? employee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");

            if (employee == null)
                return RedirectToAction("Login", "Employee");

            Order order = _orderService.GetOrCreateActiveOrder(tableId, employee);

            //Mark the table as occupied in the database
            _tableService.SetTableOccupiedStatus(tableId, true);

            //saving a full order object to session
            order.SaveToSession(HttpContext.Session);

            return RedirectToAction("Index", "MenuItem");
        }

        [HttpPost]
        public IActionResult AddItem(int menuItemId, int quantity)
        {
            MenuItem? item = _menuItemService.GetMenuItemByID(menuItemId);
            if (item == null)
            {
                TempData["Error"] = "Invalid menu item.";
                return RedirectToAction("Index", "MenuItem");
            }

            //retrieve the order to update items
            Order order = GetOrderFromSession();
            if (order.OrderId == 0)
            {
                TempData["Error"] = "No active order in session.";
                return RedirectToAction("Index", "MenuItem");
            }

            order.AddOrUpdateItem(item, quantity);
            order.SaveToSession(HttpContext.Session);

            TempData["AddedMessage"] = $"{quantity} × \"{item.Item_name}\" added!";
            return RedirectToAction("Index", "MenuItem");
        }
        [HttpGet]
        public IActionResult OrderDetails()
        {
            Order order = GetOrderFromSession();

            OrderDetailsViewModel viewModel = new OrderDetailsViewModel
            {
                OrderID = order.OrderId,
                TableNumber = order.Table.TableNumber,
                Items = order.OrderItems//list of order items
            }; 
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult SubmitOrder()
        {
            Order order = GetOrderFromSession();

            if (order.OrderId == 0)
            {
                TempData["OrderError"] = "No active order found.";
                return RedirectToAction("Overview", "Restaurant");
            }
            if (order.OrderItems.Count >0)
            {
                _orderService.FinalizeOrder(order);//Insert items and update stock
                Order.ClearFromSession(HttpContext.Session);
                TempData["OrderSuccess"] = "The order was submitted successfully!";
            }
            return RedirectToAction("Overview", "Restaurant");
        }

        //sprint 3 (matheus)
        [HttpPost]
        public IActionResult UpdateQuantity(int menuItemId, int adjustment)
        {
            Order order = GetOrderFromSession();
            order.IncreaseOrDecreaseQuantity(menuItemId, adjustment);
            order.SaveToSession(HttpContext.Session);

            return RedirectToAction("OrderDetails");
        }
        [HttpPost]
        public IActionResult MakeComment(int menuItemId, string comment)
        {
            Order order = GetOrderFromSession();
            OrderItem item = null;

            foreach (OrderItem orderItem in order.OrderItems)
            {
                if (orderItem.MenuItem.ItemId == menuItemId)
                {
                    item = orderItem;
                    break; // Stop after finding the first match
                }
            }
            if (item != null)
            {
                item.Comment = comment;
                order.SaveToSession(HttpContext.Session);
                TempData["CommentSaved"] = $"Comment for \"{item.MenuItem.Item_name}\" saved.";
            }
            return RedirectToAction("OrderDetails");
        }
        private Order GetOrderFromSession()
        {
            return Order.LoadFromSession(HttpContext.Session);
        }

        //bar or kitchen methods

        [HttpGet]
        public IActionResult Index()
        {
            //get Employee object 
            Employee? loggedInEmployee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");
            if (loggedInEmployee == null)
                return RedirectToAction("Login", "Employee");

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
                return View("AccessForbidden");
            }
        }

        [HttpGet]
        public IActionResult FinishedOrders()
        {
            Employee? loggedInEmployee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");

            if (loggedInEmployee == null)
                return RedirectToAction("Login", "Employee");

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
                return View("AccessForbidden");
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
    }

}
