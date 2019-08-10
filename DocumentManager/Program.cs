using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DocumentManager {
	public class Program {
		public static void Main(string[] args) {
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) {
			// https://github.com/aspnet/AspNetCore.Docs/blob/master/aspnetcore/host-and-deploy/linux-apache.md
			// https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-apache?view=aspnetcore-2.1
			// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-2.1
			// https://github.com/aspnet/MetaPackages/blob/master/src/Microsoft.AspNetCore/WebHost.cs#L161
			return WebHost.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((hostingContext, config) => {
					var env = hostingContext.HostingEnvironment;

					config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
						  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
				})
				.UseKestrel((builderContext, options) => {
					options.Configure(builderContext.Configuration.GetSection("Kestrel"));
				})
				.UseStartup<Startup>();
		}
	}
}
