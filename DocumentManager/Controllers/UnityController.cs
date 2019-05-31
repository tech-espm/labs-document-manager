using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocumentManager.Models;
using DocumentManager.Attributes;
using DocumentManager.Localization;

namespace DocumentManager.Controllers {
	public class UnityController : BaseController {
		[HttpGet]
		[AccessControl(Feature.UnityCreate)]
		public IActionResult Create() {
			return View("Edit", null);
		}

		[HttpGet]
		[AccessControl(Feature.UnityList)]
		public IActionResult Manage() {
			ViewBag.UnityEdit = LoggedUser.HasFeature(Feature.UnityEdit);
			return View(Unity.GetAll());
		}

		[HttpGet]
		[AccessControl(Feature.UnityEdit)]
		public IActionResult Edit(int id) {
			Unity unity = Unity.GetById(id);
			ViewBag.UnityDelete = LoggedUser.HasFeature(Feature.UnityDelete);
			return (unity == null ? ItemNotFound(Str.theUnity, Str.EditUnity) : View(unity));
		}

		[HttpPost]
		[AccessControl(Feature.UnityCreate, true)]
		public IActionResult Create(string nameEn, string namePtBr, string shortNameEn, string shortNamePtBr) {
			try {
				return Json(Unity.Create(nameEn, namePtBr, shortNameEn, shortNamePtBr));
			} catch (Exception ex) {
				return ErrorResult(ex, Str._A, Str._uma_a, Str.unity, $"{nameEn} / {namePtBr}");
			}
		}

		[HttpPost]
		[AccessControl(Feature.UnityEdit, true)]
		public IActionResult Update(int id, string nameEn, string namePtBr, string shortNameEn, string shortNamePtBr) {
			try {
				Unity unity = Unity.GetById(id);
				if (unity == null)
					return ErrorResult(Str.UnityNotFound);
				unity.Update(nameEn, namePtBr, shortNameEn, shortNamePtBr);
				return Json(unity);
			} catch (Exception ex) {
				return ErrorResult(ex, Str._A, Str._uma_a, Str.unity, $"{nameEn} / {namePtBr}");
			}
		}

		[HttpGet]
		[AccessControl(Feature.UnityDelete, true)]
		public IActionResult Delete(int id) {
			Unity unity = null;
			try {
				unity = Unity.GetById(id);
				if (unity == null)
					return ErrorResult(Str.UnityNotFound);
				unity.Delete();
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex, Str._A, Str._uma_a, Str.unity, unity?.Name.ToString());
			}
		}
	}
}
