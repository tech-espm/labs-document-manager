using DocumentManager.Utils;
using System;
using System.Collections.Generic;
using System.Net;

namespace DocumentManager.Models {
	public static class CASLogin {
		private static string ExtractValue(string xml, string tag) {
			// When it comes to CAS, we must *NEVER* use any standard
			// classes to read/traverse the XML, because what they return
			// is NOT a valid XML!!!
			int i = xml.IndexOf($"<{tag}>");
			if (i < 0)
				return null;
			i += 2 + tag.Length;
			int f = xml.IndexOf($"</{tag}>", i);
			return (f <= i) ? null : xml.Substring(i, f - i).Trim();
		}

		public static string ValidateTicket(string ticket) {
			WebClient client = new WebClient();
			string xml = client.DownloadString($"https://sso.espm.br/cas/serviceValidate?service={Uri.EscapeDataString(Config.Instance.CallbackURL)}&ticket={Uri.EscapeDataString(ticket)}");
			// INVALID_TICKET is usually returned when the user
			// refreshes a page that already had a valid ticket...
			if (xml != null && xml.IndexOf("INVALID_TICKET") < 0) {
				string id = ExtractValue(xml, "cas:uid");
				string name = ExtractValue(xml, "cas:Nome");
				if (string.IsNullOrWhiteSpace(name)) {
					name = ExtractValue(xml, "cas:nome");
					if (string.IsNullOrWhiteSpace(name)) {
						name = ExtractValue(xml, "cas:Name");
						if (string.IsNullOrWhiteSpace(name)) {
							name = ExtractValue(xml, "cas:name");
						}
					}
				}
				string email = ExtractValue(xml, "cas:emailAddress");
				if (string.IsNullOrWhiteSpace(email))
					email = ExtractValue(xml, "cas:email");
				if (!string.IsNullOrWhiteSpace(id) &&
					!string.IsNullOrWhiteSpace(name) &&
					!string.IsNullOrWhiteSpace(email)) {
					// @@@ OK!
				}
			}
			return null;
		}
	}
}
