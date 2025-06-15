using Chapeau.Models;
using Chapeau.Services;
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
                _orderService.FinalizeOrder(order);
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
    }

}
