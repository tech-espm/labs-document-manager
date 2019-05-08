using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocumentManager.Models;
using DocumentManager.Attributes;

namespace DocumentManager.Controllers {
	public class HomeController : BaseController {
		public IActionResult Index() {
			return View();
		}

		[HttpGet]
		[AccessControl(true)]
		public IActionResult Login() {
			if (LoggedUser != null)
				return Redirect("/");
			return View();
		}

		[HttpPost]
		[AccessControl(true)]
		public IActionResult Login(string userName, string password) {
			if (LoggedUser != null)
				return Redirect("/");
			try {
				LoggedUser = Models.User.Login(HttpContext, userName, password);
				if (LoggedUser == null)
					ViewBag.Message = "Usuário ou senha inválidos!";
				else
					return Redirect("/");
			} catch (Exception ex) {
				ViewBag.Message = "Erro ao efetuar o login \uD83D\uDE22 - " + ex.Message;
			}
			return View();
		}

		public IActionResult NoPermission() {
			return View();
		}

		[AccessControl(true)]
		public IActionResult Error() {
			return View();
		}
	}
}
