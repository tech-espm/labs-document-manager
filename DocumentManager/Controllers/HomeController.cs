using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocumentManager.Models;
using DocumentManager.Attributes;
using DocumentManager.Localization;

namespace DocumentManager.Controllers {
	public class HomeController : BaseController {
		public IActionResult Index() {
			return View();
		}

		[HttpGet]
		[AccessControl(true)]
		public IActionResult Login() {
			if (LoggedUser != null)
				return Redirect("~/");
			return View();
		}

		[HttpPost]
		[AccessControl(true)]
		public IActionResult Login(string userName, string password) {
			if (LoggedUser != null)
				return Redirect("~/");
			try {
				LoggedUser = Models.User.Login(HttpContext, userName, password);
				if (LoggedUser == null)
					ViewBag.Message = Str.UserOrPasswordIsInvalid;
				else
					return Redirect("~/");
			} catch (Exception ex) {
				ViewBag.Message = Str.AnErrorOccurredDuringTheLoginProcess + ex.Message;
			}
			return View();
		}

		[AccessControl(true)]
		public IActionResult Logout() {
			if (LoggedUser != null)
				LoggedUser.Logout(HttpContext);
			return Redirect("~/");
		}

		public IActionResult NoPermission() {
			return View();
		}

		public IActionResult Profile() {
			return View();
		}

		[HttpPost]
		[AccessControl(Feature.None, true)]
		public IActionResult EditProfile(string fullName, [FromBody]string picture, int languageId, string password, string newPassword, string newPassword2) {
			try {
				LoggedUser.EditProfile(HttpContext, fullName, picture, languageId, password, newPassword, newPassword2);
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex);
			}
		}

		[AccessControl(true)]
		public IActionResult Error() {
			return View();
		}

		[AccessControl(true)]
		public IActionResult Validate(string ticket) {
			// @@@ This is just a placeholder
			CASLogin.ValidateTicket(ticket);
			return RedirectToAction("Login");
		}
	}
}
