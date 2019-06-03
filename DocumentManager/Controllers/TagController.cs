using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocumentManager.Models;
using DocumentManager.Attributes;
using DocumentManager.Localization;

namespace DocumentManager.Controllers {
	public class TagController : BaseController {
		private static readonly object Lock = new object();

		[HttpGet]
		[AccessControl(Feature.TagCreate)]
		public IActionResult Create() {
			return View("Edit", null);
		}

		[HttpGet]
		[AccessControl(Feature.TagList)]
		public IActionResult Manage() {
			ViewBag.TagEdit = LoggedUser.HasFeature(Feature.TagEdit);
			return View(Tag.GetAll());
		}

		[HttpGet]
		[AccessControl(Feature.TagEdit)]
		public IActionResult Edit(int id) {
			Tag tag = Tag.GetById(id);
			ViewBag.TagDelete = LoggedUser.HasFeature(Feature.TagDelete);
			return (tag == null ? ItemNotFound(Str.theTag, Str.EditTag) : View(tag));
		}

		[HttpPost]
		[AccessControl(Feature.TagCreate, true)]
		public IActionResult Create(string nameEn, string namePtBr, [FromBody]Tag.Value[] values) {
			try {
				Tag tag;
				lock (Lock) {
					tag = Tag.Create(nameEn, namePtBr, values);
				}
				return Json(tag);
			} catch (Exception ex) {
				return ErrorResult(ex, Str._A, Str._uma_a, Str.tag, $"{nameEn} / {namePtBr}");
			}
		}

		[HttpPost]
		[AccessControl(Feature.TagEdit, true)]
		public IActionResult Update(int id, string nameEn, string namePtBr, [FromBody]Tag.Value[] values) {
			try {
				Tag tag;
				lock (Lock) {
					tag = Tag.GetById(id);
					if (tag == null)
						return ErrorResult(Str.TagNotFound);
					tag.Update(nameEn, namePtBr, values);
				}
				return Json(tag);
			} catch (Exception ex) {
				return ErrorResult(ex, Str._A, Str._uma_a, Str.tag, $"{nameEn} / {namePtBr}");
			}
		}

		[HttpGet]
		[AccessControl(Feature.TagDelete, true)]
		public IActionResult Delete(int id) {
			Tag tag = null;
			try {
				lock (Lock) {
					tag = Tag.GetById(id);
					if (tag == null)
						return ErrorResult(Str.TagNotFound);
					tag.Delete();
				}
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex, Str._A, Str._uma_a, Str.tag, tag?.Name.ToString());
			}
		}
	}
}
