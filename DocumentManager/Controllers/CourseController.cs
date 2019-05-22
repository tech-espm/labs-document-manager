using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocumentManager.Models;
using DocumentManager.Attributes;

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
			return (course == null ? ItemNotFound("o curso", "Editar Curso") : View(course));
		}

		[HttpPost]
		[AccessControl(Feature.CourseCreate, true)]
		public IActionResult Create(string name, string shortName) {
			try {
				return Json(Course.Create(name, shortName));
			} catch (Exception ex) {
				return ErrorResult(ex, "um curso", name);
			}
		}

		[HttpPost]
		[AccessControl(Feature.CourseEdit, true)]
		public IActionResult Update(int id, string name, string shortName) {
			try {
				Course course = Course.GetById(id);
				if (course == null)
					return ErrorResult("Curso não encontrado!");
				course.Update(name, shortName);
				return Json(course);
			} catch (Exception ex) {
				return ErrorResult(ex, "um curso", name);
			}
		}

		[HttpGet]
		[AccessControl(Feature.CourseDelete, true)]
		public IActionResult Delete(int id) {
			try {
				Course course = Course.GetById(id);
				if (course == null)
					return ErrorResult("Curso não encontrado!");
				course.Delete();
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex);
			}
		}
	}
}
