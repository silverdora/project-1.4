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

        public IActionResult TakeOrder(int tableId)
        {
            // For now, simulate logged-in employee (later use session)
            Employee currentEmployee = new Employee { employeeID = 1, employeeName = "Test", Role = Role.Server };

            Order newOrder = _orderService.TakeNewOrder(tableId, currentEmployee);

            // You can redirect to a view or return order info
            return RedirectToAction("OrderDetails", new { id = newOrder.OrderID });         
        }

        [HttpPost]
        public IActionResult AddItem(int orderID, int itemID, int quantity)
        {
            // Later this will call your service to save the item
            // For now, just redirect back to the order page
            return RedirectToAction("OrderDetails", new { id = orderID });
        }

        public IActionResult OrderDetails(int id)
        {
            var order = _orderService.GetOrderById(id);

            if (order == null)
            {
                return NotFound(); // Or redirect to an error page
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
