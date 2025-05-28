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

        public DummyOrderController(IDummyOrderService dummyOrderService, IEmployeeService employeeService, ITableService tableService)
        {
            _dummyOrderService = dummyOrderService;
            _employeeService = employeeService;
            _tableService = tableService;
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
            // Prepare dropdown data in ViewBag or ViewModel
            ViewBag.Employees = _employeeService.GetAllEmployee();
            ViewBag.Tables = _tableService.GetAllTables();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                _dummyOrderService.AddOrder(order);
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, repopulate dropdowns and return view
            ViewBag.Employees = _employeeService.GetAllEmployee();
            ViewBag.Tables = _tableService.GetAllTables();
            return View(order);
        }
    }

}
}
