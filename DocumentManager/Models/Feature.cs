using System;
using System.Collections.Generic;

namespace DocumentManager.Models {
	public enum Feature {
		None = 0, // Special, only used to validate the token

		// The features must be in ascending order to speed
		// up the lookup process
		DocumentCreate = 1,
		DocumentList = 2,
		DocumentEdit = 3,
		DocumentDelete = 4,

		CourseCreate = 5,
		CourseList = 6,
		CourseEdit = 7,
		CourseDelete = 8,

		Min = 1,
		Max = 8,

		// These features cannot be assigned to regular profiles, only
		// administrators can work with them! That's why their values
		// are outside the valid range.
		UserCreate = -1,
		UserList = -2,
		UserEdit = -3,
		UserDelete = -4,

		ProfileCreate = -5,
		ProfileList = -6,
		ProfileEdit = -7,
		ProfileDelete = -8
	}
}
