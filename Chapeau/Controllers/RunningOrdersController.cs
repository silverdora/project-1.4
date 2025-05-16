using System;
using System.Collections.Generic;
using Chapeau.Models;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;
namespace Chapeau.Controllers
{
	public class RunningOrdersController:Controller
	{
		private readonly IRunningOrdersService _runningOrdersService;

		public RunningOrdersController(IRunningOrdersService runningOrdersService)
		{
			_runningOrdersService = runningOrdersService;
		}

        [HttpGet]
        public IActionResult Index()
		{
            List<Order> orders = _runningOrdersService.GetAllBarOrders();
            return View(orders);
		}

        [HttpGet]
		public IActionResult GetBarOrdersByStatus(Status status)
		{
            if (status == Status.All)
            {
                var orders = _runningOrdersService.GetAllBarOrders();
                return View("Index", orders);
            }
            else
            {
                var orders = _runningOrdersService.GetBarOrdersByStatus(status);
                return View("Filtered", orders);
            }
        }

        [HttpGet]
        public IActionResult GetKitchenOrdersByStatus(Status status)
        {
			if (status == Status.All)
			{
				var orders = _runningOrdersService.GetAllKitchenOrders();
                return View("Index", orders);
            }
			else
			{
                var orders = _runningOrdersService.GetKitchenOrdersByStatus(status);
                return View("Filtered", orders);
            }
        }

        [HttpGet]
        public IActionResult ChangeOrderStatus()
        {
            return View();
            //нужен какой-то viewbag с подтверждением
        }

        [HttpPost]
        public IActionResult ChangeOrderStatus(OrderItem orderItem, int id)
        {
            _runningOrdersService.ChangeOrderStatus(orderItem, id);
            return View();
        }
    }
}

