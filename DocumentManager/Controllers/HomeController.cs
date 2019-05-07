using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocumentManager.Models;

namespace DocumentManager.Controllers {
	public class HomeController : BaseController {

		public IActionResult Index() {
			return View();
		}

		public IActionResult Login() {
			return View();
		}

		public IActionResult Error() {
			return View();
		}
	}
}
