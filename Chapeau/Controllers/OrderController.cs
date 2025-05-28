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
            // testing
            Employee currentEmployee = new Employee { employeeID = 1, employeeName = "Test", Role = Role.Server };

            Order newOrder = _orderService.TakeNewOrder(tableId, currentEmployee);

            
            return RedirectToAction("OrderDetails", new { id = newOrder.OrderID });         
        }

        [HttpPost]
        public IActionResult AddItem(int orderID, int itemID, int quantity)
        {
            // I believe I need to redirect to index
            return RedirectToAction();                                                                        //("OrderDetails", new { id = orderID });
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
