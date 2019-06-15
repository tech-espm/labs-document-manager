using DocumentManager.Localization;
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
			{ "JPG", "image/jpeg" },
			{ "JPE", "image/jpeg" },
			{ "JPEG", "image/jpeg" },
			{ "PNG", "image/png" },
			{ "ICO", "image/x-icon" },
			{ "BMP", "image/bmp" },
			{ "GIF", "image/gif" },
			{ "TIFF", "image/tiff" },
			{ "TIF", "image/tiff" },
			{ "TXT", "text/plain" },
			{ "CSV", "text/csv" },
			{ "CSS", "text/css" },
			{ "JS", "text/javascript" },
			{ "HTM", "text/html" },
			{ "HTML", "text/html" },
			{ "M3U", "audio/mpeg-url" },
			{ "3GP", "audio/3gpp" },
			{ "3GPP", "audio/3gpp" },
			{ "3GA", "audio/3ga" },
			{ "3GPA", "audio/3ga" },
			{ "M4A", "audio/mp4" },
			{ "AAC", "audio/aac" },
			{ "MP3", "audio/mpeg" },
			{ "MID", "audio/mid" },
			{ "RMI", "audio/mid" },
			{ "XMF", "audio/mobile-xmf" },
			{ "MXMF", "audio/mobile-xmf" },
			{ "RTTTL", "audio/x-rtttl" },
			{ "RTX", "audio/rtx" },
			{ "OTA", "audio/ota" },
			{ "IMY", "audio/imy" },
			{ "OGG", "audio/ogg" },
			{ "OGA", "audio/ogg" },
			{ "WAV", "audio/wav" },
			{ "MKA", "audio/x-matroska" },
			{ "FLAC", "audio/flac" },
			{ "MPEG", "video/mpeg" },
			{ "MPG", "video/mpeg" },
			{ "MPE", "video/mpeg" },
			{ "QT", "video/quicktime" },
			{ "MOV", "video/quicktime" },
			{ "AVI", "video/x-msvideo" },
			{ "MP4", "video/mp4" },
			{ "WMV", "video/x-ms-asf" },
			{ "MOVIE", "video/x-sgi-movie" },
			{ "XML", "application/xml" },
			{ "M3U8", "application/vnd.apple.mpegurl" },
			{ "JSON", "application/json" },
			{ "7Z", "application/x-7z-compressed" },
			{ "RAR", "application/x-rar-compressed" },
			{ "ZIP", "application/zip" },
			{ "ODP", "application/vnd.oasis.opendocument.presentation" },
			{ "ODS", "application/vnd.oasis.opendocument.spreadsheet" },
			{ "ODT", "application/vnd.oasis.opendocument.text" },
			{ "EPUB", "application/epub+zip" },
			{ "PDF", "application/pdf" },
			{ "PSD", "application/x-photoshop" },
			{ "DOC", "application/msword" },
			{ "DOT", "application/msword" },
			{ "DOCX", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
			{ "DOTX", "application/vnd.openxmlformats-officedocument.wordprocessingml.template" },
			{ "DOCM", "application/vnd.ms-word.document.macroEnabled.12" },
			{ "DOTM", "application/vnd.ms-word.template.macroEnabled.12" },
			{ "XLS", "application/vnd.ms-excel" },
			{ "XLT", "application/vnd.ms-excel" },
			{ "XLA", "application/vnd.ms-excel" },
			{ "XLSX", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
			{ "XLTX", "application/vnd.openxmlformats-officedocument.spreadsheetml.template" },
			{ "XLSM", "application/vnd.ms-excel.sheet.macroEnabled.12" },
			{ "XLTM", "application/vnd.ms-excel.template.macroEnabled.12" },
			{ "XLAM", "application/vnd.ms-excel.addin.macroEnabled.12" },
			{ "XLSB", "application/vnd.ms-excel.sheet.binary.macroEnabled.12" },
			{ "PPT", "application/vnd.ms-powerpoint" },
			{ "POT", "application/vnd.ms-powerpoint" },
			{ "PPS", "application/vnd.ms-powerpoint" },
			{ "PPA", "application/vnd.ms-powerpoint" },
			{ "PPTX", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
			{ "POTX", "application/vnd.openxmlformats-officedocument.presentationml.template" },
			{ "PPSX", "application/vnd.openxmlformats-officedocument.presentationml.slideshow" },
			{ "PPAM", "application/vnd.ms-powerpoint.addin.macroEnabled.12" },
			{ "PPTM", "application/vnd.ms-powerpoint.presentation.macroEnabled.12" },
			{ "POTM", "application/vnd.ms-powerpoint.template.macroEnabled.12" },
			{ "PPSM", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12" },
			{ "MDB", "application/vnd.ms-access" }
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
			MimesByExtension.TryGetValue(extension.ToUpperInvariant(), out string mime);
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

		public static string FormatSize(int size) {
			//StringBuilder builder = new StringBuilder(16);
			//string suffix;
			//if (size < 16384) {
			//	suffix = " bytes";
			//} else {
			//	size >>= 10;
			//	suffix = " KB";
			//}
			//builder.Append(size);
			//char thousands = (Str.CurrentLanguage == Str.LanguageEn ? ',' : '.');
			//for (int i = builder.Length - 3; i > 0; i -= 3)
			//	builder.Insert(i, thousands);
			//builder.Append(suffix);
			//return builder.ToString();
			if (size > 0) {
				size >>= 10;
				if (size == 0)
					size = 1;
			}
			return string.Format("{0:#,0} KB", size);
		}
	}
}
