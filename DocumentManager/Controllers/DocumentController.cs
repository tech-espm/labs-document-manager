using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocumentManager.Models;
using DocumentManager.Attributes;
using DocumentManager.Localization;

namespace DocumentManager.Controllers {
	public class DocumentController : BaseController {
		[HttpGet]
		[AccessControl(Feature.DocumentCreate)]
		public IActionResult Create() {
			return View("Edit", null);
		}

		[HttpGet]
		[AccessControl(Feature.DocumentList)]
		public IActionResult Manage() {
			ViewBag.DocumentEdit = LoggedUser.HasFeature(Feature.DocumentEdit);
			return View(Document.GetAll());
		}

		[HttpGet]
		[AccessControl(Feature.DocumentEdit)]
		public IActionResult Edit(int id) {
			Document document = Document.GetById(id, true);
			ViewBag.DocumentDelete = LoggedUser.HasFeature(Feature.DocumentDelete);
			return (document == null ? ItemNotFound(Str.theDocument, Str.EditDocument) : View(document));
		}

		[HttpPost]
		[AccessControl(Feature.DocumentCreate, true)]
		public IActionResult Create(Document.Data documentData) {
			try {
				return Json(Document.Create(documentData, Request.Form.Files.Count < 1 ? null : Request.Form.Files[0], LoggedUser));
			} catch (Exception ex) {
				return ErrorResult(ex, Str._O, Str._um_a, Str.document, documentData?.Name);
			}
		}

		[HttpPost]
		[AccessControl(Feature.DocumentEdit, true)]
		public IActionResult Update(Document.Data documentData) {
			try {
				Document document = Document.GetById(documentData == null ? 0 : documentData.Id, true);
				if (document == null)
					return ErrorResult(Str.DocumentNotFound);
				document.Update(documentData);
				return Json(document);
			} catch (Exception ex) {
				return ErrorResult(ex, Str._O, Str._um_a, Str.document, documentData?.Name);
			}
		}

		[HttpGet]
		[AccessControl(Feature.DocumentDelete, true)]
		public IActionResult Delete(int id) {
			Document document = null;
			try {
				document = Document.GetById(id, false);
				if (document == null)
					return ErrorResult(Str.DocumentNotFound);
				document.Delete();
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex, Str._O, Str._um_a, Str.document, document?.Name.ToString());
			}
		}

		[HttpGet]
		[AccessControl(Feature.DocumentList)]
		public IActionResult View(int id) {
			Document document = null;
			try {
				document = Document.GetById(id, false);
				if (document == null)
					return ErrorResult(Str.DocumentNotFound);
				return FileResult(Storage.Document(document.Id, document.Extension));
			} catch (Exception ex) {
				return ErrorResult(ex);
			}
		}

		[HttpGet]
		[AccessControl(Feature.DocumentList)]
		public IActionResult Download(int id) {
			Document document = null;
			try {
				document = Document.GetById(id, false);
				if (document == null)
					return ErrorResult(Str.DocumentNotFound);
				return DownloadResult(Storage.Document(document.Id, document.Extension), document.SafeDownloadName);
			} catch (Exception ex) {
				return ErrorResult(ex);
			}
		}
	}
}
