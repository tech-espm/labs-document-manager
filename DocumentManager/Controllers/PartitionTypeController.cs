using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocumentManager.Models;
using DocumentManager.Attributes;

namespace DocumentManager.Controllers {
	public class PartitionTypeController : BaseController {
		[HttpGet]
		[AccessControl(Feature.PartitionTypeCreate)]
		public IActionResult Create() {
			return View("Edit", null);
		}

		[HttpGet]
		[AccessControl(Feature.PartitionTypeList)]
		public IActionResult Manage() {
			ViewBag.PartitionTypeEdit = LoggedUser.HasFeature(Feature.PartitionTypeEdit);
			return View(PartitionType.GetAll());
		}

		[HttpGet]
		[AccessControl(Feature.PartitionTypeEdit)]
		public IActionResult Edit(int id) {
			PartitionType partitionType = PartitionType.GetById(id);
			ViewBag.PartitionTypeDelete = LoggedUser.HasFeature(Feature.PartitionTypeDelete);
			return (partitionType == null ? ItemNotFound("o tipo de partição", "Editar Tipo de Partição") : View(partitionType));
		}

		[HttpPost]
		[AccessControl(Feature.PartitionTypeCreate, true)]
		public IActionResult Create(string name) {
			try {
				return Json(PartitionType.Create(name));
			} catch (Exception ex) {
				return ErrorResult(ex, "o", "um", "tipo de partição", name);
			}
		}

		[HttpPost]
		[AccessControl(Feature.PartitionTypeEdit, true)]
		public IActionResult Update(int id, string name) {
			try {
				PartitionType partitionType = PartitionType.GetById(id);
				if (partitionType == null)
					return ErrorResult("Tipo de partição não encontrado!");
				partitionType.Update(name);
				return Json(partitionType);
			} catch (Exception ex) {
				return ErrorResult(ex, "o", "um", "tipo de partição", name);
			}
		}

		[HttpGet]
		[AccessControl(Feature.PartitionTypeDelete, true)]
		public IActionResult Delete(int id) {
			PartitionType partitionType = null;
			try {
				partitionType = PartitionType.GetById(id);
				if (partitionType == null)
					return ErrorResult("Tipo de partição não encontrado!");
				partitionType.Delete();
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex, "o", "um", "tipo de partição", partitionType?.Name);
			}
		}
	}
}
