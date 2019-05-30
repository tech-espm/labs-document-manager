using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocumentManager.Models;
using DocumentManager.Attributes;
using DocumentManager.Localization;

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
			return (profile == null ? ItemNotFound(Str.theProfile, Str.EditProfile) : View(profile));
		}

		[HttpPost]
		[AccessControl(Feature.ProfileCreate, true)]
		public IActionResult Create(string nameEn, string namePtBr, [FromBody]int[] features) {
			try {
				return Json(Profile.Create(nameEn, namePtBr, features));
			} catch (Exception ex) {
				return ErrorResult(ex, Str._O, Str._um_a, Str.profile, $"{nameEn} / {namePtBr}");
			}
		}

		[HttpPost]
		[AccessControl(Feature.ProfileEdit, true)]
		public IActionResult Update(int id, string nameEn, string namePtBr, [FromBody]int[] features) {
			try {
				Profile profile = Profile.GetById(id, false);
				if (profile == null)
					return ErrorResult(Str.ProfileNotFound);
				profile.Update(nameEn, namePtBr, features);
				return Json(profile);
			} catch (Exception ex) {
				return ErrorResult(ex, Str._O, Str._um_a, Str.profile, $"{nameEn} / {namePtBr}");
			}
		}

		[HttpGet]
		[AccessControl(Feature.ProfileDelete, true)]
		public IActionResult Delete(int id) {
			Profile profile = null;
			try {
				profile = Profile.GetById(id, false);
				if (profile == null)
					return ErrorResult(Str.ProfileNotFound);
				profile.Delete();
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex, Str._O, Str._um_a, Str.profile, profile?.Name.ToString());
			}
		}
	}
}
