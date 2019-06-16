using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocumentManager.Models;
using DocumentManager.Attributes;
using DocumentManager.Localization;

namespace DocumentManager.Controllers {
	public class UserController : BaseController {
		[HttpGet]
		[AccessControl(Feature.UserCreate)]
		public IActionResult Create() {
			return View("Edit");
		}

		[HttpGet]
		[AccessControl(Feature.UserList)]
		public IActionResult Manage() {
			return View(Models.User.GetAllWithProfileName());
		}

		[HttpPost]
		[AccessControl(Feature.UserCreate, true)]
		public IActionResult Create(string userName, string fullName, int profileId, int languageId) {
			try {
				return Json(Models.User.Create(userName, fullName, profileId, languageId));
			} catch (Exception ex) {
				return ErrorResult(ex, Str._o, Str._um_a, Str.user, userName, Str.theUserName);
			}
		}

		[HttpGet]
		[AccessControl(Feature.UserEdit, true)]
		public IActionResult SetProfile(int id, int profileId) {
			try {
				LoggedUser.SetProfile(id, profileId);
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex);
			}
		}

        [HttpDelete]
        public IActionResult DeletePartitionTypePermission(int id)
        {
            try
            {
                Models.User.PartitionTypePermission partitionTypePermission = new User.PartitionTypePermission();

                var result = new
                {
                    success = partitionTypePermission.Delete(id)

                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return ErrorResult(ex);
            }
        }

        [HttpDelete]
        public IActionResult DeleteDocumentTypePermission(int id)
        {
            try
            {
                Models.User.DocumentTypePermission documentTypePermission = new User.DocumentTypePermission();

                var result = new
                {
                    success = documentTypePermission.Delete(id)

                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return ErrorResult(ex);
            }
        }

        [HttpPost]
        public IActionResult AddDocumentTypePermission(int UserId, int UnityId, int CourseId, int DocumentTypeId, int FeaturePermissionId)
        {
            try
            {
                Models.User.DocumentTypePermission documentTypePermission = new User.DocumentTypePermission();

                var result = new
                {
                    success = documentTypePermission.Add(UserId, UnityId, CourseId, DocumentTypeId, FeaturePermissionId)

                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return ErrorResult(ex);
            }
        }

        [HttpPost]
        public IActionResult AddPartitionTypePermission(int UserId, int UnityId, int CourseId, int PartitionTypeId, int FeaturePermissionId)
        {
            try
            {
                Models.User.PartitionTypePermission partitionTypePermission = new User.PartitionTypePermission();

                var result = new
                {
                    success = partitionTypePermission.Add(UserId, UnityId, CourseId, PartitionTypeId, FeaturePermissionId)

                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return ErrorResult(ex);
            }
        }

        [HttpGet]
        public IActionResult GetPartitionTypePermission(int UserID)
        {
            try
            {
                return Json(Models.User.PartitionTypePermission.GetPermissions(UserID));
            }
            catch (Exception ex)
            {
                return ErrorResult(ex);
            }
        }

        [HttpGet]
        public IActionResult GetDocumentTypePermission(int UserID)
        {
            try
            {
                return Json(Models.User.DocumentTypePermission.GetPermissions(UserID));
            }
            catch (Exception ex)
            {
                return ErrorResult(ex);
            }
        }


        [HttpGet, HttpHead]
		public IActionResult Picture(int id, int v) {
			try {
				return Models.User.Picture(HttpContext, id);
			} catch (Exception ex) {
				return ErrorResult(ex);
			}
		}

		[HttpGet]
		[AccessControl(Feature.UserEdit, true)]
		public IActionResult Activate(int id) {
			try {
				LoggedUser.Activate(id);
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex);
			}
		}

		[HttpGet]
		[AccessControl(Feature.UserEdit, true)]
		public IActionResult Deactivate(int id) {
			try {
				LoggedUser.Deactivate(id);
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex);
			}
		}

		[HttpGet]
		[AccessControl(Feature.UserEdit, true)]
		public IActionResult ResetPassword(int id) {
			try {
				LoggedUser.ResetPassword(id);
				return VoidResult();
			} catch (Exception ex) {
				return ErrorResult(ex);
			}
		}
	}
}
