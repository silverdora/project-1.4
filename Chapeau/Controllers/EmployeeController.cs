using Microsoft.AspNetCore.Mvc;
using Chapeau.Models;
using Chapeau.Service.Interface;
using Chapeau.Models.Extensions;

namespace Chapeau.Controllers
{
    public class EmployeeController : Controller
    {
        private IEmployeeService _employeeService;

        // مُنشئ الكائن - يستقبل خدمة الموظفين عبر الحقن (Dependency Injection)
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // عرض صفحة تسجيل الدخول (GET)
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // يُرجع صفحة تسجيل الدخول
        }

        // تنفيذ عملية تسجيل الدخول بعد إرسال النموذج (POST)
        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            // محاولة الحصول على موظف باستخدام بيانات تسجيل الدخول
            Employee? employee = _employeeService.GetByLoginCredentials(loginModel.UserName, loginModel.Password);

            // إذا لم يتم العثور على الموظف، عرض رسالة خطأ
            if (employee == null)
            {
                ViewBag.ErrorMessage = "اسم المستخدم أو كلمة المرور غير صحيحة!";
                return View(loginModel);
            }

            // تخزين معلومات الموظف في الجلسة بعد تسجيل الدخول بنجاح
            HttpContext.Session.SetObject("LoggedInEmployee", employee);

            // توجيه المستخدم حسب الدور (الصلاحية)
            switch (employee.Role)
            {
                case Role.Server:
                    return RedirectToAction("Overview", "Restaurant"); // نادل
                case Role.Bar:
                    return RedirectToAction("Overview", "Bar"); // موظف البار
                case Role.Kitchen:
                    return RedirectToAction("Overview", "Kitchen"); // المطبخ
                case Role.Manager:
                    return RedirectToAction("Dashboard", "Manager"); // المدير
                default:
                    return RedirectToAction("Login"); // في حالة دور غير معروف، العودة لصفحة الدخول
            }
        }

        // تنفيذ تسجيل الخروج
        [HttpPost]
        public IActionResult LogOff()
        {
            HttpContext.Session.Clear(); // مسح الجلسة
            TempData["LoggedOutMessage"] = "تم تسجيل الخروج بنجاح.";
            return RedirectToAction("Login", "Employee"); // الرجوع لصفحة الدخول
        }

        // عرض قائمة الموظفين (للاستخدام الإداري غالباً)
        public IActionResult Index()
        {
            // الحصول على الموظف المسجّل حالياً من الجلسة
            Employee? loggedInEmployee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");
            ViewData["loggedInEmployee"] = loggedInEmployee;

            // الحصول على جميع الموظفين من قاعدة البيانات
            List<Employee> employees = _employeeService.GetAllEmployee();
            return View(employees); // عرضهم في الواجهة
        }
    }
}
