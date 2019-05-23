using Microsoft.AspNetCore.Mvc;
using DocumentManager.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;
using DocumentManager.Attributes;
using System.Text;
using DocumentManager.Exceptions;
using MySql.Data.MySqlClient;

namespace DocumentManager.Controllers {
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class BaseController : Controller {
		protected User LoggedUser;

		protected ActionResult VoidResult() => new StatusCodeResult(204);

		protected ActionResult ErrorResult(string message) {
			ContentResult result = Content($"{{\"ExceptionMessage\":\"{(message ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n").Replace('\0', ' ')}\"}}", "application/json", Encoding.UTF8);
			result.StatusCode = 299;
			return result;
		}

		protected ActionResult ErrorResult(Exception ex) {
			if (ex is ValidationException)
				return ErrorResult(ex.Message);
			if (ex is MySqlException)
				return ErrorResult($"Ocorreu o erro 0x{ex.HResult.ToString("X8")} na base de dados: {ex.Message}");
			return ErrorResult($"Ocorreu o erro 0x{ex.HResult.ToString("X8")} no servidor: {ex.Message}");
		}

		protected ActionResult ErrorResult(Exception ex, string definiteArticle, string undefiniteArticle, string itemName, string value, string articlePlusFieldName = null) {
			MySqlException myex = ex as MySqlException;
			if (myex != null) {
				if (myex.Number == 1062)
					return ErrorResult($"Já existe {undefiniteArticle} {itemName} com {(articlePlusFieldName ?? "o nome")} \"{(value ?? "").ToUpper()}\" \uD83D\uDE22");
				else if (myex.Number == 1451)
					return ErrorResult($"{definiteArticle.ToUpper()} {itemName} \"{(value ?? "").ToUpper()}\" possui dependências e não pode ser excluíd{definiteArticle} \uD83D\uDE22");
			}
			return ErrorResult(ex);
		}

		protected ActionResult ItemNotFound(string articlePlusItemName, string title) {
			ViewBag.Title = title;
			ViewBag.ItemName = articlePlusItemName;
			return View("_NotFound");
		}

		public override void OnActionExecuting(ActionExecutingContext context) {
			LoggedUser = Models.User.GetFromClient(HttpContext);
			ViewBag.LoggedUser = LoggedUser;

			ControllerActionDescriptor actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
			object[] attributes = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(AccessControlAttribute), true);
			Feature requestedFeature = Feature.None;
			bool apiCall = false;
			if (attributes != null && attributes.Length > 0) {
				AccessControlAttribute accessControl = attributes[0] as AccessControlAttribute;
				if (accessControl.Anonymous) {
					base.OnActionExecuting(context);
					return;
				}
				requestedFeature = accessControl.RequestedFeature;
				apiCall = accessControl.ApiCall;
			}

			if (LoggedUser == null || !LoggedUser.HasFeature(requestedFeature)) {
				if (apiCall)
					context.Result = ErrorResult("Sem permissão");
				else if (LoggedUser != null)
					context.Result = View("~/Views/Home/NoPermission.cshtml");
				else if (string.IsNullOrWhiteSpace(Request.Path) || Request.Path == "/")
					context.Result = View("~/Views/Home/Login.cshtml");
				else
					context.Result = Redirect("/Home/Login/?r=" + Uri.EscapeDataString(Request.Scheme + "://" + Request.Host + Request.Path + Request.QueryString.ToUriComponent()));
				return;
			}

			base.OnActionExecuting(context);
		}

		public ActionResult Error(string message) => new ContentResult() {
			Content = message,
			ContentType = "text/plain",
			StatusCode = 500
		};
	}
}
