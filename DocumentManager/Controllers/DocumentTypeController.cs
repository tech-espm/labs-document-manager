using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocumentManager.Models;
using DocumentManager.Attributes;

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
			return (documentType == null ? ItemNotFound("o tipo de documento", "Editar Tipo de Documento") : View(documentType));
		}

		[HttpPost]
		[AccessControl(Feature.DocumentTypeCreate, true)]
		public IActionResult Create(string name) {
			try {
				return Json(DocumentType.Create(name));
			} catch (Exception ex) {
				return ErrorResult(ex, "o", "um", "tipo de documento", name);
			}
		}

		[HttpPost]
		[AccessControl(Feature.DocumentTypeEdit, true)]
		public IActionResult Update(int id, string name) {
			try {
				DocumentType documentType = DocumentType.GetById(id);
				if (documentType == null)
					return ErrorResult("Tipo de documento não encontrado!");
				documentType.Update(name);
				return Json(documentType);
			} catch (Exception ex) {
				return ErrorResult(ex, "o", "um", "tipo de documento", name);
			}
		}

		[HttpGet]
		[AccessControl(Feature.DocumentTypeDelete, true)]
		public IActionResult Delete(int id) {
			DocumentType documentType = null;
			try {
				documentType = DocumentType.GetById(id);
				if (documentType == null)
					return ErrorResult("Tipo de documento não encontrado!");
				documentType.Delete();
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex, "o", "um", "tipo de documento", documentType?.Name);
			}
		}
	}
}
