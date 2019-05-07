using Microsoft.AspNetCore.Mvc;
using DocumentManager.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DocumentManager.Controllers {
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class BaseController : Controller {

		public string AppData => AppDomain.CurrentDomain.GetData("DataDirectory").ToString();

		public string AppDataFilePath(string folder, int id, string extension) => System.IO.Path.Combine(AppData, folder, id + "." + extension);

		public IActionResult Error(string message) => new ContentResult() {
			Content = message,
			ContentType = "text/plain",
			StatusCode = 500
		};
	}
}
