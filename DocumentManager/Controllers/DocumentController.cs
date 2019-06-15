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
			ViewBag.DocumentDelete = LoggedUser.HasFeature(Feature.DocumentDelete);
			return View();
		}

		[HttpPost]
		[AccessControl(Feature.DocumentList, true)]
		public IActionResult GetAllByFilter(Document.Data documentData) {
			return Json(Document.GetAllByFilter(documentData, LoggedUser));
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
		[Route("Document/{id}/{fileName}")]
		[AccessControl(Feature.DocumentList)]
		public IActionResult View(int id, string fileName) {
			try {
				Document document = Document.GetById(id, false);
				if (document == null)
					return ErrorResult(Str.DocumentNotFound);
				return FileResult(Storage.Document(document.Id, document.Extension));
			} catch (Exception ex) {
				return ErrorResult(ex);
			}
		}

		[HttpGet]
		[Route("Document/Download/{id}/{fileName}")]
		[AccessControl(Feature.DocumentList)]
		public IActionResult Download(int id, string fileName) {
			try {
				Document document = Document.GetById(id, false);
				if (document == null)
					return ErrorResult(Str.DocumentNotFound);
				return DownloadResult(Storage.Document(document.Id, document.Extension), string.IsNullOrWhiteSpace(fileName) ? document.SafeDownloadName : fileName);
			} catch (Exception ex) {
				return ErrorResult(ex);
			}
		}

		[HttpGet]
		[Route("Document/DownloadSelected/{ids}/{fileName}")]
		[AccessControl(Feature.DocumentList)]
		public IActionResult DownloadSelected(string ids, string fileName) {
			try {
				if (string.IsNullOrWhiteSpace(ids))
					return ErrorResult(Str.DocumentNotFound);
				string[] splitIds = ids.Split('|');
				string[] names = new string[splitIds.Length];
				for (int i = splitIds.Length - 1; i >= 0; i--) {
					if (!int.TryParse(splitIds[i], out int id))
						return ErrorResult(Str.DocumentNotFound);
					Document document = Document.GetById(id, false);
					if (document == null)
						return ErrorResult(Str.DocumentNotFound);
					splitIds[i] = Storage.Document(id, document.Extension);
					names[i] = document.SafeDownloadName;
				}
				return DownloadZipResult(splitIds, names, string.IsNullOrWhiteSpace(fileName) ? (Str.SelectedDocuments + ".zip") : fileName);
			} catch (Exception ex) {
				return ErrorResult(ex);
			}
		}
	}
}
