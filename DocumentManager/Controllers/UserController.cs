using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocumentManager.Models;
using DocumentManager.Attributes;

namespace DocumentManager.Controllers {
	public class UserController : BaseController {
		[HttpGet]
		[AccessControl(Feature.UserCreate)]
		public IActionResult Create() {
			return View("Edit");
		}

		[HttpGet]
		[AccessControl(Feature.UserList)]
		public IActionResult Manage() {
			return View(Models.User.GetAllWithProfileName());
		}

		[HttpPost]
		[AccessControl(Feature.UserCreate, true)]
		public IActionResult Create(string userName, string fullName, int profileId) {
			try {
				return Json(Models.User.Create(userName, fullName, profileId));
			} catch (Exception ex) {
				return ErrorResult(ex, "um usuário", userName, "login");
			}
		}

		[HttpGet]
		[AccessControl(Feature.UserEdit, true)]
		public IActionResult SetProfile(int id, int profileId) {
			try {
				LoggedUser.SetProfile(id, profileId);
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex);
			}
		}

		[HttpGet]
		[AccessControl(Feature.UserEdit, true)]
		public IActionResult Activate(int id) {
			try {
				LoggedUser.Activate(id);
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex);
			}
		}

		[HttpGet]
		[AccessControl(Feature.UserEdit, true)]
		public IActionResult Deactivate(int id) {
			try {
				LoggedUser.Deactivate(id);
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex);
			}
		}

		[HttpGet]
		[AccessControl(Feature.UserEdit, true)]
		public IActionResult ResetPassword(int id) {
			try {
				LoggedUser.ResetPassword(id);
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex);
			}
		}
	}
}
