using Chapeau.Models;
using Chapeau.Services;

using Chapeau.HelperMethods;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult TakeOrder(int tableId, int employeeId)
        {
            Order newOrder = new Order
            {
                Table = new Table { TableId = tableId },
                Employee = new Employee { employeeID = employeeId },
                OrderTime = DateTime.Now,
                IsServed = false
            };

            _orderService.InsertOrder(newOrder); // Save to DB

            TempData["CurrentOrderId"] = newOrder.OrderID; // Store order ID for future use
            return RedirectToAction("SelectItems", new { orderId = newOrder.OrderID, tableId = tableId });
        }
        //public IActionResult SelectItems()
        //{
        //    List<MenuItem> menuItems = _menuItemService.GetMenuItems();
        //    return View(menuItems);
        //}

        [HttpPost]
        public IActionResult AddItemToSelection(int menuItemId, int quantity)
        {
            _orderService.AddItemToSessionSelection(menuItemId, quantity, HttpContext.Session);
            return RedirectToAction("Index", "MenuItem");
            //return RedirectToAction("SelectItems", new { orderId = orderId });
        }
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
        public IActionResult SubmitOrder(int orderId)
        {
            List<OrderItem> selectedItems = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("SelectedItems");

            if (selectedItems != null && selectedItems.Count > 0)
            {
                _orderService.AddItemsToOrder(orderId, selectedItems);

                // Clear session after saving to DB
                HttpContext.Session.Remove("SelectedItems");
            }

            return RedirectToAction("Tables", "Table");
        }

        //[HttpPost]
        //public IActionResult AddItemsToOrder(int orderId)
        //{
        //    List<OrderItem> selectedItems = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("SelectedItems");

        //    if (selectedItems != null && selectedItems.Count > 0)
        //    {
        //        _orderService.AddItemsToOrder(orderId, selectedItems);
        //        HttpContext.Session.Remove("SelectedItems"); // clear after submitting
        //    }

        //    return RedirectToAction("Tables", "Table"); // or wherever you go next
        //}








      // // public IActionResult SelectItems()
      //  {
      //      // display all menu items with buttons
      //      return View(menuItemService.GetAll());
      //  }
      //  //Add item to selection (session list)
      //  [HttpPost]
      ////  public IActionResult AddItem(int menuItemId)
      //  {
      //      orderService.AddItemToSessionSelection(menuItemId, HttpContext.Session);
      //      return RedirectToAction("SelectItems");
      //  }

      //  //Show selected items
      //  public IActionResult OrderDetails()
      //  {
      //      var items = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("SelectedItems");
      //      return View(items);
      //  }

      //  //Confirm and add to database
      //  [HttpPost]
      //  public IActionResult AddItemsToOrder()
      //  {
      //      int orderId = (int)TempData["CurrentOrderId"];
      //      var items = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("SelectedItems");

      //      orderService.AddItemsToOrder(orderId, items);

      //      HttpContext.Session.Remove("SelectedItems");
      //      return RedirectToAction("Tables");
      //  }

        //public IActionResult TakeOrder(int tableId)
        //{
        //    // Just pass tableId forward, do not create order yet
        //    return RedirectToAction("Index", "MenuItem", new { tableID = tableId });
        //}
        //[HttpPost]
        //public IActionResult AddItem(int orderID, int itemID, int quantity, int tableID, string? card, string? category)
        //{
        //    // Simulated employee – replace with logged-in user later
        //    Employee currentEmployee = new Employee
        //    {
        //        employeeID = 1,
        //        employeeName = "Test",
        //        Role = Role.Server
        //    };

        //    // Create a new order if orderID is 0
        //    if (orderID == 0)
        //    {
        //        Order newOrder = _orderService.TakeNewOrder(tableID, currentEmployee);
        //        orderID = newOrder.OrderID;
        //    }

        //    // Add the item to the order
        //    _orderService.AddSingleItemToOrder(orderID, itemID, quantity);

        //    // Redirect back to MenuItem with updated orderID and tableID
        //    return RedirectToAction("Index", "MenuItem", new
        //    {
        //        orderID = orderID,
        //        tableID = tableID,
        //        card = card,
        //        category = category
        //    });
        //}


        //public IActionResult OrderDetails(int id)
        //{
        //    var order = _orderService.GetOrderById(id);

        //    if (order == null)
        //    {
        //        return NotFound(); 
        //    }

        //    var viewModel = new OrderDetailsViewModel
        //    {
        //        OrderID = order.OrderID,
        //        EmployeeName = order.Employee.employeeName,
        //        TableNumber = order.Table.TableNumber,
        //        OrderTime = order.OrderTime,
        //        Items = order.OrderItems
        //    };

        //    return View(viewModel);
        //}      
    }
}
