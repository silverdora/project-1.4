using System;
using Chapeau.Models;
using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;
namespace Chapeau.Controllers
{
	public class RunningOrdersController:Controller
	{
		private readonly IMenuItemService _menuItemService;

		public RunningOrdersController()
		{
		}
	}
}

