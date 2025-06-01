using Chapeau.Models;
using Chapeau.Service.Interface;
using Chapeau.Services;
using Chapeau.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class DummyOrderController : Controller
    {

        private readonly IDummyOrderService _dummyOrderService;
        private readonly IEmployeeService _employeeService;  // To get employees
        private readonly ITableService _tableService;
        private readonly IMenuItemService _menuItemService;

        public DummyOrderController(IDummyOrderService dummyOrderService, IEmployeeService employeeService, ITableService tableService, IMenuItemService menuItemService)
        {
            _dummyOrderService = dummyOrderService;
            _employeeService = employeeService;
            _tableService = tableService;
            _menuItemService = menuItemService;
        }

        // GET: /DummyOrder/Index
        public IActionResult Index()
        {
            var orders = _dummyOrderService.GetAllOrders();
            return View("~/Views/DummyOrder/Index.cshtml", orders);
        }


        // Optional: Details action
        public IActionResult Details(int id)
        {
            var order = _dummyOrderService.GetOrderById(id);
            if (order == null)
                return NotFound();

            return View(order);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Employees = _employeeService.GetAllEmployee();
            ViewBag.Tables = _tableService.GetAllTables();
            ViewBag.MenuItems = _menuItemService.GetAllMenuItems();

            return View(new Order { OrderItems = new List<OrderItem>() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                // Load full Employee and Table objects
                order.Employee = _employeeService.GetAllEmployee()
                    .FirstOrDefault(e => e.employeeID == order.Employee.employeeID);
                order.Table = _tableService.GetAllTables()
                    .FirstOrDefault(t => t.TableNumber == order.Table.TableNumber);

                // For each OrderItem, load the full MenuItem and set IncludeDate & Status
                foreach (var oi in order.OrderItems)
                {
                    oi.MenuItem = _menuItemService.GetAllMenuItems()
                        .FirstOrDefault(mi => mi.ItemID == oi.ItemID);
                    oi.IncludeDate = DateTime.Now;
                    oi.Status = Status.InProgress; // or appropriate default
                }

                _dummyOrderService.AddOrder(order);

                return RedirectToAction(nameof(Index));
            }

            // If model invalid, reload dropdowns
            ViewBag.Employees = _employeeService.GetAllEmployee();
            ViewBag.Tables = _tableService.GetAllTables();
            ViewBag.MenuItems = _menuItemService.GetAllMenuItems();
            return View(order);
        }
    }
}

