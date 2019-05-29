using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocumentManager.Models;
using DocumentManager.Attributes;
using DocumentManager.Localization;

namespace DocumentManager.Controllers {
	public class CourseController : BaseController {
		[HttpGet]
		[AccessControl(Feature.CourseCreate)]
		public IActionResult Create() {
			return View("Edit", null);
		}

		[HttpGet]
		[AccessControl(Feature.CourseList)]
		public IActionResult Manage() {
			ViewBag.CourseEdit = LoggedUser.HasFeature(Feature.CourseEdit);
			return View(Course.GetAll());
		}

		[HttpGet]
		[AccessControl(Feature.CourseEdit)]
		public IActionResult Edit(int id) {
			Course course = Course.GetById(id);
			ViewBag.CourseDelete = LoggedUser.HasFeature(Feature.CourseDelete);
			return (course == null ? ItemNotFound(Str.theCourse, Str.EditCourse) : View(course));
		}

		[HttpPost]
		[AccessControl(Feature.CourseCreate, true)]
		public IActionResult Create(string nameEn, string namePtBr, string shortNameEn, string shortNamePtBr) {
			try {
				return Json(Course.Create(nameEn, namePtBr, shortNameEn, shortNamePtBr));
			} catch (Exception ex) {
				return ErrorResult(ex, Str._O, Str._um_a, Str.course, $"{nameEn} / {namePtBr}");
			}
		}

		[HttpPost]
		[AccessControl(Feature.CourseEdit, true)]
		public IActionResult Update(int id, string nameEn, string namePtBr, string shortNameEn, string shortNamePtBr) {
			try {
				Course course = Course.GetById(id);
				if (course == null)
					return ErrorResult(Str.CourseNotFound);
				course.Update(nameEn, namePtBr, shortNameEn, shortNamePtBr);
				return Json(course);
			} catch (Exception ex) {
				return ErrorResult(ex, Str._O, Str._um_a, Str.course, $"{nameEn} / {namePtBr}");
			}
		}

		[HttpGet]
		[AccessControl(Feature.CourseDelete, true)]
		public IActionResult Delete(int id) {
			try {
				Course course = Course.GetById(id);
				if (course == null)
					return ErrorResult(Str.CourseNotFound);
				course.Delete();
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex);
			}
		}
	}
}
