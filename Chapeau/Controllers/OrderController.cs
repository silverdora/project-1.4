using Chapeau.Models;
using Chapeau.Services;
using Chapeau.HelperMethods;
using Microsoft.AspNetCore.Mvc;
using Chapeau.Models.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Chapeau.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMenuItemService _menuItemService;
       

        public OrderController(IOrderService orderService, IMenuItemService menuItemService)
        {
            _orderService = orderService;
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
                Table = new Chapeau.Models.Table { TableId = tableId },//to avoid ambiguous reference with name Table
                Employee = employee,
                OrderTime = DateTime.Now,
                IsServed = false,
                IsPaid = false,
                IsReadyToPay = false
            };

            _orderService.InsertOrder(newOrder);

            // ✅ Store in session (NOT TempData)
            HttpContext.Session.SetInt32("CurrentOrderId", newOrder.OrderID);
            HttpContext.Session.SetInt32("CurrentTableId", tableId);

            // ✅ Redirect to MenuItem with valid IDs
            return RedirectToAction("Index", "MenuItem", new
            {
                orderId = newOrder.OrderID,
                tableId = tableId
            });            
        }
       
        [HttpPost]
        public IActionResult AddItem(int menuItemId, int quantity)
        {
            int? orderId = HttpContext.Session.GetInt32("CurrentOrderId");
            int? tableId = HttpContext.Session.GetInt32("CurrentTableId");

            if (orderId == null || tableId == null)
            {
                TempData["Error"] = "No active order found. Please start a new order.";
                return RedirectToAction("Tables", "Table");
            }

            _orderService.AddItemToSessionSelection(menuItemId, quantity, HttpContext.Session);

            MenuItem item = _menuItemService.GetMenuItemByID(menuItemId);
            TempData["AddedMessage"] = $"{quantity} × \"{item.Item_name}\" added successfully!";

            return RedirectToAction("Index", "MenuItem", new
            {
                orderId = orderId.Value,
                tableId = tableId.Value
            });
        }
        [HttpGet]
        public IActionResult OrderDetails()
        {            

            List<OrderItem> selectedItems = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("SelectedItems");
            if (selectedItems == null)
            {
                selectedItems = new List<OrderItem>();
            }

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

                HttpContext.Session.Remove("SelectedItems");

                // Optional: Clear current order/table
                HttpContext.Session.Remove("CurrentOrderId");
                HttpContext.Session.Remove("CurrentTableId");

                TempData["OrderSuccess"] = "The order was submitted successfully!";
            }

            return RedirectToAction("Overview", "Restaurant");
        }
    }
}
