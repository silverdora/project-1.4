using Chapeau.Models;
using Chapeau.Services;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public IActionResult TakeOrder(int tableId)
        {
            // Just pass tableId forward, do not create order yet
            return RedirectToAction("Index", "MenuItem", new { tableID = tableId });
        }

        [HttpPost]
        public IActionResult AddItem(int orderID, int itemID, int quantity, int tableID)
        {
            // Simulated employee – replace with logged-in user later
            Employee currentEmployee = new Employee
            {
                employeeID = 1,
                employeeName = "Test",
                Role = Role.Server
            };

            // Create a new order if orderID is 0
            if (orderID == 0)
            {
                Order newOrder = _orderService.TakeNewOrder(tableID, currentEmployee);
                orderID = newOrder.OrderID;
            }

            // Add the item to the order
            _orderService.AddSingleItemToOrder(orderID, itemID, quantity);

            // Redirect back to MenuItem with updated orderID and tableID
            return RedirectToAction("Index", "MenuItem", new { orderID = orderID, tableID = tableID });
        }


        public IActionResult OrderDetails(int id)
        {
            var order = _orderService.GetOrderById(id);

            if (order == null)
            {
                return NotFound(); 
            }

            var viewModel = new OrderDetailsViewModel
            {
                OrderID = order.OrderID,
                EmployeeName = order.Employee.employeeName,
                TableNumber = order.Table.TableNumber,
                OrderTime = order.OrderTime,
                Items = order.OrderItems
            };

            return View(viewModel);
        }      
    }
}
