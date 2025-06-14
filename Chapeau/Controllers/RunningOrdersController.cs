using System;
using System.Collections.Generic;
using Chapeau.Models;
using Chapeau.Models.Extensions;
using Chapeau.Service.Interface;
using Chapeau.Services;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static NuGet.Packaging.PackagingConstants;

namespace Chapeau.Controllers
{
    public class RunningOrdersController : Controller
    {
        private readonly IRunningOrdersService _runningOrdersService;
        private readonly IEmployeeService _employeeService;

        public RunningOrdersController(IRunningOrdersService runningOrdersService, IEmployeeService employeeService)
        {
            _runningOrdersService = runningOrdersService;
            _employeeService = employeeService;

        }

        
        [HttpGet]
        public IActionResult Index()
        {
            //get Employee object 
            Employee? loggedInEmployee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");
            if (loggedInEmployee == null)
            {
                throw new Exception("no user");
            }

            if ((loggedInEmployee.Role == Role.Bar) || (loggedInEmployee.Role == Role.Kitchen))
            {

                string type = (loggedInEmployee.Role == Role.Bar) ? "Drink" : "Dish";
                List<Order> newOrders = _runningOrdersService.GetOrdersByStatus(Status.Ordered, type);
                Dictionary<int, List<MenuCategory>> newOrdersByCourse = _runningOrdersService.GetCategoriesOfAnOrder(newOrders);

                List<Order> preparingOrders = _runningOrdersService.GetOrdersByStatus(Status.InProgress, type);
                Dictionary<int, List<MenuCategory>> preparingOrdersByCourse = _runningOrdersService.GetCategoriesOfAnOrder(preparingOrders);

                //store data in the ready orders ViewModel
                RunningOrdersViewModel runningOrdersViewModel = new RunningOrdersViewModel(newOrders, preparingOrders, newOrdersByCourse, preparingOrdersByCourse, loggedInEmployee);
                //pass data to view
                return View(runningOrdersViewModel);
            }

            else
            {
                 throw new Exception("no access");
            }
        }

        [HttpGet]
        public IActionResult FinishedOrders()
        {
            //get Employee object 
            Employee? loggedInEmployee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");
            
            if (loggedInEmployee == null)
            {
                throw new Exception("no user");
            }

            
            if ((loggedInEmployee.Role == Role.Bar ) || (loggedInEmployee.Role == Role.Kitchen))
            {
                 
                string type = (loggedInEmployee.Role == Role.Bar) ? "Drink" : "Dish";
                List<Order>  readyOrders = _runningOrdersService.GetOrdersByStatus(Status.ReadyToBeServed, type);
                List<Order> servedOrders = _runningOrdersService.GetOrdersByStatus(Status.Served, type);
                Dictionary<int, List<MenuCategory>> readyOrdersByCourse = _runningOrdersService.GetCategoriesOfAnOrder(readyOrders);
                Dictionary<int, List<MenuCategory>> servedOrdersByCourse = _runningOrdersService.GetCategoriesOfAnOrder(servedOrders);
                //store data in the ready orders ViewModel
                ReadyToBeServedOrdersViewModel toBeServedOrdersViewModel = new ReadyToBeServedOrdersViewModel(readyOrders, servedOrders, readyOrdersByCourse, servedOrdersByCourse, loggedInEmployee);
                //pass data to view
                return View(toBeServedOrdersViewModel);
            }

            else
            {
                throw new Exception("no access");
            }
            
        }

        [HttpPost]
        public IActionResult ChangeOrderItemStatus(int orderItemID, Status status)
        {
            _runningOrdersService.ChangeOrderStatus(orderItemID, status);
            TempData["StatusChangeMessage"] = "Status has been changed.";
            //go back
            if (status == Status.Served)
            {
                return RedirectToAction("FinishedOrders");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ChangeAllOrderItemsStatus(int orderID, Status currentStatus, Status newStatus)
        {
            Employee? loggedInEmployee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");
            if (loggedInEmployee.Role == Role.Kitchen)
            {
                _runningOrdersService.ChangeAllOrderItemsStatus(orderID, "Dish", currentStatus, newStatus);
            }
            else
            {
                _runningOrdersService.ChangeAllOrderItemsStatus(orderID, "Drink", currentStatus, newStatus);
            }
            
            //go back
            TempData["StatusChangeMessage"] = "Status has been changed.";
            if (newStatus == Status.Served)
            {
                return RedirectToAction("FinishedOrders");
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult ChangeOrderItemsFromOneCourseStatus(int orderID, Status currentStatus, Status newStatus, MenuCategory course)
        {
            _runningOrdersService.ChangeOrderItemsFromOneCourseStatus(orderID, currentStatus, newStatus, course);
            //go back
            TempData["StatusChangeMessage"] = "Status has been changed.";
            if (newStatus == Status.Served)
            {
                return RedirectToAction("FinishedOrders");
            }
            return RedirectToAction("Index");
        }
    }
}
