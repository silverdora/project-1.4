using System;
using System.Collections.Generic;
using Chapeau.Models;
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

		public RunningOrdersController(IRunningOrdersService runningOrdersService)
		{
			_runningOrdersService = runningOrdersService;
		}

        //for the time when log in implementation is not available, only kitchen orders are displayed
        [HttpGet]
        public IActionResult Index()
		{
            //user needs to be logged in
            //Employee? employee = HttpContext.Session.GetObject<Employee>("LoggedInUser");
            //if (employee == null)
            //{
            //    throw new Exception("no access");
            //}

            ////get Employee object via employees repository
            //Employee? employee = _usersService.GetUserById(employee.employeeID);



            //get all orders
            //List<Order>? orders = new List<Order>();
            //if (employee.Role == Enumerations.Role.Bar)
            //{
            //    orders = _runningOrdersService.GetAllBarOrders();
            //}
            //else if (employee.Role == Enumerations.Role.Kitchen)
            //{
            //    orders = _runningOrdersService.GetAllKitchenOrders();
            //}
            //else
            //{
            //    throw new Exception("no access");
            //}

            //List<Order> orders = _runningOrdersService.GetAllKitchenOrders();
            List<Order> newOrders = _runningOrdersService.GetKitchenOrdersByStatus(Status.New);
            List<Order> preparingOrders = _runningOrdersService.GetKitchenOrdersByStatus(Status.InProgress);
            List<Order> readyOrders = _runningOrdersService.GetKitchenOrdersByStatus(Status.Ready);
            //store data in the running orders ViewModel
            RunningOrdersViewModel runningOrdersViewModel = new RunningOrdersViewModel(newOrders, preparingOrders, readyOrders);

            //pass data to view
            return View(runningOrdersViewModel);
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

