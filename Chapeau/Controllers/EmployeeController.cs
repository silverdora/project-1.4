using Microsoft.AspNetCore.Mvc;
using Chapeau.Models;
using Chapeau.Service.Interface;
using Chapeau.Models.Extensions;

namespace Chapeau.Controllers
{
    public class EmployeeController : Controller
    {
        private  IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            Employee? employee = _employeeService.GetByLoginCredentials(loginModel.UserName, loginModel.Password);

            if (employee == null)
            {
                ViewBag.ErrorMessage = "Bad username/password combination!";
                return View(loginModel);
            }

            HttpContext.Session.SetObject("LoggedInEmployee", employee);

            switch (employee.Role)
            {
                case Role.Server:
                    return RedirectToAction("Overview", "Restaurant");
                case Role.Bar:
                    return RedirectToAction("Overview", "Bar");
                case Role.Kitchen:
                    return RedirectToAction("Index", "RunningOrders");
                case Role.Manager:
                    return RedirectToAction("Dashboard", "Manager");
                default:
                    return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public IActionResult LogOff()
        {
            HttpContext.Session.Clear();
            TempData["LoggedOutMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login", "Employee");
        }

        public IActionResult Index()
        {
            Employee? loggedInEmployee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");
            ViewData["loggedInEmployee"] = loggedInEmployee;

            List<Employee> employees = _employeeService.GetAllEmployee();
            return View(employees);
        }
    }
}
