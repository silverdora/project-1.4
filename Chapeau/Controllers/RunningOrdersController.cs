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

        //for the time when log in implementation is not available, only kitchen orders are displayed
        [HttpGet]
        public IActionResult Index()
        {
            //get Employee object 
            Employee? loggedInEmployee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");
            if (loggedInEmployee == null)
            {
                throw new Exception("no user");
            }

            if (loggedInEmployee.Role == Role.Bar)
            {
                List<Order> newOrders = _runningOrdersService.GetBarOrdersByStatus(Status.Ordered);
                Dictionary<int, List<MenuCategory>> newOrdersByCourse = _runningOrdersService.GetCategoriesOfAnOrder(newOrders);

                List<Order> preparingOrders = _runningOrdersService.GetBarOrdersByStatus(Status.InProgress);
                Dictionary<int, List<MenuCategory>> preparingOrdersByCourse = _runningOrdersService.GetCategoriesOfAnOrder(preparingOrders);

                //store data in the running orders ViewModel
                RunningOrdersViewModel runningOrdersViewModel = new RunningOrdersViewModel(newOrders, preparingOrders, newOrdersByCourse, preparingOrdersByCourse, loggedInEmployee);
                //pass data to view
                return View(runningOrdersViewModel);
            }


            else if (loggedInEmployee.Role == Role.Kitchen)
            {

                List<Order> newOrders = _runningOrdersService.GetKitchenOrdersByStatus(Status.Ordered);
                Dictionary<int, List<MenuCategory>> newOrdersByCourse = _runningOrdersService.GetCategoriesOfAnOrder(newOrders);

                List<Order> preparingOrders = _runningOrdersService.GetKitchenOrdersByStatus(Status.InProgress);
                Dictionary<int, List<MenuCategory>> preparingOrdersByCourse = _runningOrdersService.GetCategoriesOfAnOrder(preparingOrders);


                //store data in the running orders ViewModel
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
        public IActionResult ReadyToBeServed()
        {
            //get Employee object 
            Employee? loggedInEmployee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");
            if (loggedInEmployee == null)
            {
                throw new Exception("no user");
            }

            if (loggedInEmployee.Role == Role.Bar)
            {
                List<Order> readyOrders = _runningOrdersService.GetBarOrdersByStatus(Status.ReadyToBeServed);
                Dictionary<int, List<MenuCategory>> readyOrdersByCourse = _runningOrdersService.GetCategoriesOfAnOrder(readyOrders);

                //store data in the ready orders ViewModel
                ReadyToBeServedOrdersViewModel toBeServedOrdersViewModel = new ReadyToBeServedOrdersViewModel(readyOrders, readyOrdersByCourse, loggedInEmployee);
                //pass data to view
                return View(toBeServedOrdersViewModel);
            }


            else if (loggedInEmployee.Role == Role.Kitchen)
            {
                List<Order> readyOrders = _runningOrdersService.GetKitchenOrdersByStatus(Status.ReadyToBeServed);
                Dictionary<int, List<MenuCategory>> readyOrdersByCourse = _runningOrdersService.GetCategoriesOfAnOrder(readyOrders);

                //store data in the ready orders ViewModel
                ReadyToBeServedOrdersViewModel toBeServedOrdersViewModel = new ReadyToBeServedOrdersViewModel(readyOrders, readyOrdersByCourse, loggedInEmployee);
                //pass data to view
                return View(toBeServedOrdersViewModel);
            }

            else
            {
                throw new Exception("no access");
            }
        }

        [HttpPost]
        public IActionResult ChangeOrderItemStatus(int orderID, int itemID, Status status)
        {
            _runningOrdersService.ChangeOrderStatus(orderID, itemID, status);
            TempData["StatusChangeMessage"] = "Status has been changed.";
            //go back
            if (status == Status.Served)
            {
                return RedirectToAction("ReadyToBeServed");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ChangeAllOrderItemsStatus(int orderID, Status currentStatus, Status newStatus)
        {
            _runningOrdersService.ChangeAllOrderItemsStatus(orderID, currentStatus, newStatus);
            //go back
            TempData["StatusChangeMessage"] = "Status has been changed.";
            if (newStatus == Status.Served)
            {
                return RedirectToAction("ReadyToBeServed");
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
                return RedirectToAction("ReadyToBeServed");
            }
            return RedirectToAction("Index");
        }
    }
}

