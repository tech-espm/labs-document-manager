using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace DocumentManager.Utils {
	public static class JS {
		// In order to convert dates from C# to JavaScript, we must subtract
		// 621355968000000000 (1970/1/1 00:00) from the C# ticks, as 1970 is the
		// origin for JavaScript dates
		public const long JSDateOrigin = 621355968000000000;
		public const int TicksToMS = 10000;

		private static readonly string[] Months = { "/JAN/", "/FEV/", "/MAR/", "/ABR/", "/MAI/", "/JUN/", "/JUL/", "/AGO/", "/SET/", "/OUT/", "/NOV/", "/DEZ/" };
		private static readonly string[] MonthsWithoutYear = { "/JAN", "/FEV", "/MAR", "/ABR", "/MAI", "/JUN", "/JUL", "/AGO", "/SET", "/OUT", "/NOV", "/DEZ" };

		// *******************************************************************************************
		// MOST PLACES (IN FACT, ALL PLACES, EXCEPT FOR THE ADVERTISEMENT LOG) WHERE UTC IS READ,
		// THE DATE ACTUALLY CONTAINS LOCAL TIME! UTC IS USED JUST TO PREVENT ANY CONVERSIONS!
		// *******************************************************************************************

		public static long ToJS(DateTime dateTime) => (long)((ulong)(dateTime.Ticks - JSDateOrigin) / TicksToMS);
		//public static long ToJS(DateTime dateTime) {
		//	//return (long)((ulong)(dateTime.ToUniversalTime().Ticks - JSDateOrigin) / TicksToMS);
		//	return (long)((ulong)(dateTime.Ticks - JSDateOrigin) / TicksToMS);
		//	//return (long)(dateTime.Second | (dateTime.Minute << 6) | (dateTime.Hour << 12) | (dateTime.Day << 17) | (dateTime.Month << 22)) | ((long)dateTime.Year << 26);
		//}

		public static DateTime FromJS(long jsDateTimeUTC) => new DateTime((jsDateTimeUTC * TicksToMS) + JSDateOrigin, DateTimeKind.Utc);
		//public static DateTime FromJS(long jsDateTimeUTC) {
		//	//0x4000000000000000L = (long)System.DateTimeKind.Utc << 62
		//	//DateTime.FromBinary(0x4000000000000000L).AddTicks((jsDateTimeUTC * TicksToMS) + JSDateOrigin)
		//	//return new DateTimeOffset((jsDateTimeUTC * TicksToMS) + JSDateOrigin, TimeSpan.Zero);
		//	return new DateTime((jsDateTimeUTC * TicksToMS) + JSDateOrigin, DateTimeKind.Utc);
		//	//int tmp = (int)jsDateTimeUTC;
		//	//return new DateTime((int)(jsDateTimeUTC >> 26), (tmp >> 22) & 15, (tmp >> 17) & 31, (tmp >> 12) & 31, (tmp >> 6) & 63, tmp & 63);
		//}

		public static string Format(object x) {
			//return x.ToString().Replace("\\", "\\\\").Replace("\"", "\\u0022").Replace("\'", "\\u0027").Replace("\r", "").Replace("\n", "\\n").Replace("<", "\\u003C").Replace(">", "\\u003E");
			return HttpUtility.JavaScriptStringEncode(x.ToString());
		}

		public static string RemoveTags(object x) {
			return x.ToString().Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
		}
	}
}
