using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace DocumentManager.Utils {
	// Config instead of Configuration in order to avoid conflicts with the framework classes
	public class Config {
		// Without dependency injection ;)

		public string CallbackURL { get; set; }

		private static Config instance;

		public static Config Instance => (instance ?? (instance = Load()));

		private static Config Load() {
			IConfigurationBuilder builder = new ConfigurationBuilder()
						.SetBasePath(Directory.GetCurrentDirectory())
						.AddJsonFile("appsettings.json", false, true)
						.AddEnvironmentVariables();

			IConfiguration config = builder.Build().GetSection("DocumentManager");

			return new Config() {
				CallbackURL = config.GetValue<string>("CallbackURL")
			};
		}
	}
}
