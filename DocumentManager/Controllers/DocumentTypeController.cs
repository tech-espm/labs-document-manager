using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocumentManager.Models;
using DocumentManager.Attributes;
using DocumentManager.Localization;

namespace DocumentManager.Controllers {
	public class DocumentTypeController : BaseController {
		[HttpGet]
		[AccessControl(Feature.DocumentTypeCreate)]
		public IActionResult Create() {
			return View("Edit", null);
		}

		[HttpGet]
		[AccessControl(Feature.DocumentTypeList)]
		public IActionResult Manage() {
			ViewBag.DocumentTypeEdit = LoggedUser.HasFeature(Feature.DocumentTypeEdit);
			return View(DocumentType.GetAll());
		}

		[HttpGet]
		[AccessControl(Feature.DocumentTypeEdit)]
		public IActionResult Edit(int id) {
			DocumentType documentType = DocumentType.GetById(id);
			ViewBag.DocumentTypeDelete = LoggedUser.HasFeature(Feature.DocumentTypeDelete);
			return (documentType == null ? ItemNotFound(Str.theDocumentType, Str.EditDocumentType) : View(documentType));
		}

		[HttpPost]
		[AccessControl(Feature.DocumentTypeCreate, true)]
		public IActionResult Create(string nameEn, string namePtBr) {
			try {
				return Json(DocumentType.Create(nameEn, namePtBr));
			} catch (Exception ex) {
				return ErrorResult(ex, Str._O, Str._um_a, Str.documentType, $"{nameEn} / {namePtBr}");
			}
		}

		[HttpPost]
		[AccessControl(Feature.DocumentTypeEdit, true)]
		public IActionResult Update(int id, string nameEn, string namePtBr) {
			try {
				DocumentType documentType = DocumentType.GetById(id);
				if (documentType == null)
					return ErrorResult(Str.DocumentTypeNotFound);
				documentType.Update(nameEn, namePtBr);
				return Json(documentType);
			} catch (Exception ex) {
				return ErrorResult(ex, Str._O, Str._um_a, Str.documentType, $"{nameEn} / {namePtBr}");
			}
		}

		[HttpGet]
		[AccessControl(Feature.DocumentTypeDelete, true)]
		public IActionResult Delete(int id) {
			DocumentType documentType = null;
			try {
				documentType = DocumentType.GetById(id);
				if (documentType == null)
					return ErrorResult(Str.DocumentTypeNotFound);
				documentType.Delete();
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex, Str._O, Str._um_a, Str.documentType, documentType?.Name.ToString());
			}
		}
	}
}
