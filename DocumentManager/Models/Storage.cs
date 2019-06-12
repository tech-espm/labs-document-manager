using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentManager.Models {
	public static class Storage {
		public static string WWWRoot => AppDomain.CurrentDomain.GetData("WWWRootDirectory").ToString();
		public static string AppData => AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
		public static readonly string DefaultMime = "application/octet-stream";

		#region MimesByExtension
		private static readonly Dictionary<string, string> MimesByExtension = new Dictionary<string, string>() {
			{ "jpg", "image/jpeg" },
			{ "jpe", "image/jpeg" },
			{ "jpeg", "image/jpeg" },
			{ "png", "image/png" },
			{ "ico", "image/x-icon" },
			{ "bmp", "image/bmp" },
			{ "gif", "image/gif" },
			{ "tiff", "image/tiff" },
			{ "tif", "image/tiff" },
			{ "txt", "text/plain" },
			{ "csv", "text/csv" },
			{ "css", "text/css" },
			{ "js", "text/javascript" },
			{ "htm", "text/html" },
			{ "html", "text/html" },
			{ "m3u", "audio/mpeg-url" },
			{ "3gp", "audio/3gpp" },
			{ "3gpp", "audio/3gpp" },
			{ "3ga", "audio/3ga" },
			{ "3gpa", "audio/3ga" },
			{ "mp4", "audio/mp4" },
			{ "m4a", "audio/mp4" },
			{ "aac", "audio/aac" },
			{ "mp3", "audio/mpeg" },
			{ "mid", "audio/mid" },
			{ "rmi", "audio/mid" },
			{ "xmf", "audio/mobile-xmf" },
			{ "mxmf", "audio/mobile-xmf" },
			{ "rtttl", "audio/x-rtttl" },
			{ "rtx", "audio/rtx" },
			{ "ota", "audio/ota" },
			{ "imy", "audio/imy" },
			{ "ogg", "audio/ogg" },
			{ "oga", "audio/ogg" },
			{ "wav", "audio/wav" },
			{ "mka", "audio/x-matroska" },
			{ "flac", "audio/flac" },
			{ "mpeg", "video/mpeg" },
			{ "mpg", "video/mpeg" },
			{ "mpe", "video/mpeg" },
			{ "qt", "video/quicktime" },
			{ "mov", "video/quicktime" },
			{ "avi", "video/x-msvideo" },
			{ "mp4", "video/mp4" },
			{ "wmv", "video/x-ms-asf" },
			{ "movie", "video/x-sgi-movie" },
			{ "xml", "application/xml" },
			{ "m3u8", "application/vnd.apple.mpegurl" },
			{ "json", "application/json" },
			{ "7z", "application/x-7z-compressed" },
			{ "rar", "application/x-rar-compressed" },
			{ "zip", "application/zip" },
			{ "odp", "application/vnd.oasis.opendocument.presentation" },
			{ "ods", "application/vnd.oasis.opendocument.spreadsheet" },
			{ "odt", "application/vnd.oasis.opendocument.text" },
			{ "epub", "application/epub+zip" },
			{ "pdf", "application/pdf" },
			{ "psd", "application/x-photoshop" },
			{ "doc", "application/msword" },
			{ "dot", "application/msword" },
			{ "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
			{ "dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template" },
			{ "docm", "application/vnd.ms-word.document.macroEnabled.12" },
			{ "dotm", "application/vnd.ms-word.template.macroEnabled.12" },
			{ "xls", "application/vnd.ms-excel" },
			{ "xlt", "application/vnd.ms-excel" },
			{ "xla", "application/vnd.ms-excel" },
			{ "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
			{ "xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template" },
			{ "xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12" },
			{ "xltm", "application/vnd.ms-excel.template.macroEnabled.12" },
			{ "xlam", "application/vnd.ms-excel.addin.macroEnabled.12" },
			{ "xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12" },
			{ "ppt", "application/vnd.ms-powerpoint" },
			{ "pot", "application/vnd.ms-powerpoint" },
			{ "pps", "application/vnd.ms-powerpoint" },
			{ "ppa", "application/vnd.ms-powerpoint" },
			{ "pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
			{ "potx", "application/vnd.openxmlformats-officedocument.presentationml.template" },
			{ "ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow" },
			{ "ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12" },
			{ "pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12" },
			{ "potm", "application/vnd.ms-powerpoint.template.macroEnabled.12" },
			{ ".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12" },
			{ ".mdb", "application/vnd.ms-access" }
		};
		#endregion

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

		public static string Document(int id, string extension) => AppDataFilePath("Documents", id, extension);

		public static string Mime(string extension) {
			if (string.IsNullOrWhiteSpace(extension))
				return null;
			MimesByExtension.TryGetValue(extension.ToLowerInvariant(), out string mime);
			return mime;
		}

		public static string SafeFileName(string fileName) {
			// After benchmarking, chained Replace() calls have proved to be faster only
			// if they do not replace any characters!
			//
			// Not replacing a single character (1M iterations):
			// Chained Replace(): 1500 ms
			// Compiled Regex (new Regex(..., RegexOptions.Compiled)): 1600 ms
			// StringBuilder (calling ToLower() for each character): 1800 ms
			// StringBuilder (calling ToLower() before): 780 ms
			//
			// Replacing characters (1M iterations):
			// Chained Replace(): 3900 ms
			// Compiled Regex (new Regex(..., RegexOptions.Compiled)): 8900 ms
			// StringBuilder (calling ToLower() for each character): 2000 ms
			// StringBuilder (calling ToLower() before): 1000 ms

			//return fileName.Replace('.', '_').Replace('/', '_').Replace('?', '_').Replace('*', '_').Replace('\\', '_').Replace('<', '_').Replace('>', '_').Replace('{', '_').Replace('}', '_').Replace('$', '_').Replace('!', '_').Replace('~', '_').Replace('%', '_').Replace(':', '_').Replace(';', '_').Replace(',', '_').Replace('|', '_').Replace('\"', '_').Replace('\'', '_').Replace('`', '_');

			int total = fileName.Length;
			StringBuilder builder = new StringBuilder(total);
			for (int i = 0; i < total; i++) {
				char c = fileName[i];
				switch (c) {
					case '.':
					case '/':
					case '?':
					case '*':
					case '\\':
					case '<':
					case '>':
					case '{':
					case '}':
					case '$':
					case '!':
					case '~':
					case '%':
					case ':':
					case ';':
					case ',':
					case '|':
					case '\"':
					case '\'':
					case '`':
						builder.Append('_');
						break;
					default:
						builder.Append((c < 32) ? '_' : c);
						break;
				}
			}
			return builder.ToString();
		}
	}
}
