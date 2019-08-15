using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace DocumentManager.Utils {
	public class AppSetting {
		public static AppSetting GetAppSetting() {
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.Development.json", optional: true)
				.AddJsonFile($"appsettings.Production.json", optional: true)
				.AddEnvironmentVariables();

			return builder.Build().Get<AppSetting>();
		}

		public string MySQL { get; set; }
		public string PathBase { get; set; }
	}
}
