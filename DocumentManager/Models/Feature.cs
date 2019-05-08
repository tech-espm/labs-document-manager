using System;
using System.Collections.Generic;

namespace DocumentManager.Models {
	public enum Feature {
		None = 0, // Special, only used to validate the token

		// The features must be in ascending order and must be in
		// the same order they appear on NavBar.ascx (to speed up the
		// lookup process)
		DocumentCreate = 1,
		DocumentList = 2,
		DocumentEdit = 3,
		DocumentDelete = 4,

		// There are no features for profiles and users because only
		// administrators can work with them!

		Min = 1,
		Max = 4
	}
}
