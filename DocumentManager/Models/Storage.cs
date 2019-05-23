using System;
using System.Collections.Generic;

namespace DocumentManager.Models {
	public static class Storage {
		public static string WWWRoot => AppDomain.CurrentDomain.GetData("WWWRootDirectory").ToString();
		public static string AppData => AppDomain.CurrentDomain.GetData("DataDirectory").ToString();

		public static string GenerateETag(string file) {
			return System.IO.File.GetLastWriteTimeUtc(file).ToString("yyyyMMddHHmmss");
		}

		public static Microsoft.Net.Http.Headers.EntityTagHeaderValue GenerateFullETag(string file, out DateTime lastModifiedUtc) {
			DateTime l;
			string etag = (l = System.IO.File.GetLastWriteTimeUtc(file)).ToString("yyyyMMddHHmmss");
			lastModifiedUtc = new DateTime(l.Year, l.Month, l.Day, l.Hour, l.Minute, l.Second, DateTimeKind.Utc);
			return new Microsoft.Net.Http.Headers.EntityTagHeaderValue($"\"{etag}\"");
		}

		public static string AppDataFilePath(string folder, int id, string extension) => System.IO.Path.Combine(AppData, folder, id + "." + extension);

		public static string UserProfilePicture(int id) => AppDataFilePath("Profiles", id, "jpg");
	}
}
