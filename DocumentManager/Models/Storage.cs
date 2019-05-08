using System;
using System.Collections.Generic;

namespace DocumentManager.Models {
	public static class Storage {
		public static string AppData => AppDomain.CurrentDomain.GetData("DataDirectory").ToString();

		public static string AppDataFilePath(string folder, int id, string extension) => System.IO.Path.Combine(AppData, folder, id + "." + extension);

		public static string UserProfilePicture(int id) => AppDataFilePath("Profiles", id, "jpg");
	}
}
