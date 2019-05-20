using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocumentManager.Models;
using DocumentManager.Attributes;

namespace DocumentManager.Controllers {
	public class ProfileController : BaseController {
		[HttpGet]
		[AccessControl(Feature.ProfileCreate)]
		public IActionResult Create() {
			return View("Edit", null);
		}

		[HttpGet]
		[AccessControl(Feature.ProfileList)]
		public IActionResult Manage() {
			ViewBag.ProfileEdit = LoggedUser.HasFeature(Feature.ProfileEdit);
			return View(Profile.GetAll());
		}

		[HttpGet]
		[AccessControl(Feature.ProfileEdit)]
		public IActionResult Edit(int id) {
			Profile profile = Profile.GetById(id, true);
			ViewBag.ProfileDelete = LoggedUser.HasFeature(Feature.ProfileDelete);
			return (profile == null ? ItemNotFound("o perfil", "Editar Perfil") : View(profile));
		}

		[HttpPost]
		[AccessControl(Feature.ProfileCreate, true)]
		public IActionResult Create(string name, [FromBody]int[] features) {
			try {
				return Json(Profile.Create(name, features));
			} catch (Exception ex) {
				return ErrorResult(ex, "um perfil", name);
			}
		}

		[HttpPost]
		[AccessControl(Feature.ProfileEdit, true)]
		public IActionResult Update(int id, string name, [FromBody]int[] features) {
			try {
				Profile p = Profile.GetById(id, false);
				if (p == null)
					return ErrorResult("Perfil não encontrado!");
				p.Update(name, features);
				return Json(p);
			} catch (Exception ex) {
				return ErrorResult(ex, "um perfil", name);
			}
		}

		[HttpGet]
		[AccessControl(Feature.ProfileDelete, true)]
		public IActionResult Delete(int id) {
			try {
				Profile p = Profile.GetById(id, false);
				if (p == null)
					return ErrorResult("Perfil não encontrado!");
				p.Delete();
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex);
			}
		}
	}
}
