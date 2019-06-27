using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocumentManager.Models;
using DocumentManager.Attributes;
using DocumentManager.Localization;

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
			return (partitionType == null ? ItemNotFound(Str.thePartitionType, Str.EditPartitionType) : View(partitionType));
		}

		[HttpPost]
		[AccessControl(Feature.PartitionTypeCreate, true)]
		public IActionResult Create(string nameEn, string namePtBr) {
			try {
				return Json(PartitionType.Create(nameEn, namePtBr));
			} catch (Exception ex) {
				return ErrorResult(ex, Str._A, Str._uma_a, Str.partitionType, $"{nameEn} / {namePtBr}");
			}
		}

		[HttpPost]
		[AccessControl(Feature.PartitionTypeEdit, true)]
		public IActionResult Update(int id, string nameEn, string namePtBr) {
			try {
				PartitionType partitionType = PartitionType.GetById(id);
				if (partitionType == null)
					return ErrorResult(Str.PartitionTypeNotFound);
				partitionType.Update(nameEn, namePtBr);
				return Json(partitionType);
			} catch (Exception ex) {
				return ErrorResult(ex, Str._A, Str._uma_a, Str.partitionType, $"{nameEn} / {namePtBr}");
			}
		}

		[HttpGet]
		[AccessControl(Feature.PartitionTypeDelete, true)]
		public IActionResult Delete(int id) {
			PartitionType partitionType = null;
			try {
				partitionType = PartitionType.GetById(id);
				if (partitionType == null)
					return ErrorResult(Str.PartitionTypeNotFound);
				partitionType.Delete();
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex, Str._A, Str._uma_a, Str.partitionType, partitionType?.Name.ToString());
			}
		}
	}
}
