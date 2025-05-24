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
	public class RunningOrdersController:Controller
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
                throw new Exception("no access");
            }

            if (loggedInEmployee.Role == Role.Bar)
            {
                List<Order> newOrders = _runningOrdersService.GetKitchenOrdersByStatus(Status.New);
                List<Order> preparingOrders = _runningOrdersService.GetKitchenOrdersByStatus(Status.InProgress);
                List<Order> readyOrders = _runningOrdersService.GetKitchenOrdersByStatus(Status.Ready);
                //store data in the running orders ViewModel
                RunningOrdersViewModel runningOrdersViewModel = new RunningOrdersViewModel(newOrders, preparingOrders, readyOrders, loggedInEmployee);
                //pass data to view
                return View(runningOrdersViewModel);
            }
            
            else if (loggedInEmployee.Role == Role.Kitchen)
            {
                List<Order> newOrders = _runningOrdersService.GetKitchenOrdersByStatus(Status.New);
                List<Order> preparingOrders = _runningOrdersService.GetKitchenOrdersByStatus(Status.InProgress);
                List<Order> readyOrders = _runningOrdersService.GetKitchenOrdersByStatus(Status.Ready);
                //store data in the running orders ViewModel
                RunningOrdersViewModel runningOrdersViewModel = new RunningOrdersViewModel(newOrders, preparingOrders, readyOrders, loggedInEmployee);
                //pass data to view
                return View(runningOrdersViewModel);
            }
            else
            {
                throw new Exception("no access");
            }
		}



  //      [HttpGet]
		//public IActionResult Filtered(Status status)
		//{
  //          if (status == Status.All)
  //          {
  //              var orders = _runningOrdersService.GetAllBarOrders();
  //              return View("Index", orders);
  //          }
  //          else
  //          {
  //              var orders = _runningOrdersService.GetBarOrdersByStatus(status);
  //              return View("Filtered", orders);
  //          }
  //      }

        [HttpGet]
        public IActionResult Filtered(Status status)
        {
			//if (status == Status.All)
			//{
   //             List<Order> newOrders = _runningOrdersService.GetKitchenOrdersByStatus(Status.New);
   //             List<Order> preparingOrders = _runningOrdersService.GetKitchenOrdersByStatus(Status.InProgress);
   //             List<Order> readyOrders = _runningOrdersService.GetKitchenOrdersByStatus(Status.Ready);

   //             RunningOrdersViewModel runningOrdersViewModel = new RunningOrdersViewModel(newOrders, preparingOrders, readyOrders);

   //             //pass data to view
   //             return View("Index", runningOrdersViewModel);
   //         }
			//else
			//{
                List<Order> orders = _runningOrdersService.GetKitchenOrdersByStatus(status);
                FilteredOrdersViewModel filteredOrdersViewModel = new FilteredOrdersViewModel(orders, status);
                return View(filteredOrdersViewModel);
            //}
        }

        [HttpPost]
        public IActionResult ChangeOrderStatus(int itemID, Status status)
        {
            _runningOrdersService.ChangeOrderStatus(itemID, status);

            

            //go back 
            return RedirectToAction("Index");
        }
    }
}

