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
using DocumentManager.Localization;

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
			if (Str.CurrentLanguage == Str.LanguageEn) {
				if (ex is MySqlException)
					return ErrorResult($"Database error 0x{ex.HResult.ToString("X8")}: {ex.Message}");
				return ErrorResult($"Server error 0x{ex.HResult.ToString("X8")}: {ex.Message}");
			} else {
				if (ex is MySqlException)
					return ErrorResult($"Ocorreu o erro 0x{ex.HResult.ToString("X8")} na base de dados: {ex.Message}");
				return ErrorResult($"Ocorreu o erro 0x{ex.HResult.ToString("X8")} no servidor: {ex.Message}");
			}
		}

		protected ActionResult ErrorResult(Exception ex, string upperCaseDefiniteArticle, string undefiniteArticle, string itemName, string value, string fieldName = null) {
			if (ex is MySqlException myex) {
				if (Str.CurrentLanguage == Str.LanguageEn) {
					if (myex.Number == 1062)
						return ErrorResult($"There is already {undefiniteArticle} {itemName} with {(fieldName ?? "name")} \"{(value ?? "").ToUpper()}\" \uD83D\uDE22");
					else if (myex.Number == 1451)
						return ErrorResult($"{upperCaseDefiniteArticle} {itemName} \"{(value ?? "").ToUpper()}\" has dependencies, so it cannot be delete \uD83D\uDE22");
				} else {
					if (myex.Number == 1062)
						return ErrorResult($"Já existe {undefiniteArticle} {itemName} com {(fieldName ?? "nome")} \"{(value ?? "").ToUpper()}\" \uD83D\uDE22");
					else if (myex.Number == 1451)
						return ErrorResult($"{upperCaseDefiniteArticle} {itemName} \"{(value ?? "").ToUpper()}\" possui dependências e não pode ser excluíd{upperCaseDefiniteArticle.ToLower()} \uD83D\uDE22");
				}
			}
			return ErrorResult(ex);
		}

		protected ActionResult ItemNotFound(string articlePlusItemName, string title) {
			ViewBag.Title = title;
			ViewBag.ItemName = articlePlusItemName;
			return View("_NotFound");
		}

		protected ActionResult FileResult(string path, string extensionOverride = null) {
			if (extensionOverride == null)
				extensionOverride = System.IO.Path.GetExtension(path).ToLowerInvariant().Substring(1);
			return new PhysicalFileResult(path, Storage.Mime(extensionOverride) ?? Storage.DefaultMime);
		}

		protected ActionResult DownloadResult(string path, string downloadName = null, string extensionOverride = null) {
			ActionResult result = FileResult(path, extensionOverride);
			Response.Headers.Add("Content-Disposition", "attachment; filename=" + (downloadName ?? System.IO.Path.GetFileName(path)));
			return result;
		}

		public override void OnActionExecuting(ActionExecutingContext context) {
			LoggedUser = Models.User.GetFromClient(HttpContext);
			ViewBag.LoggedUser = LoggedUser;

			Str.SetCurrentLanguage(LoggedUser == null ? Str.LanguagePtBr : LoggedUser.LanguageId);

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
					context.Result = ErrorResult(Str.NoPermission);
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
