using DocumentManager.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;

namespace DocumentManager {
	public class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {

			var appSetting = AppSetting.GetAppSetting();

			services.AddSingleton(instance => appSetting);

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env) {

			var appSetting = AppSetting.GetAppSetting();

			app.UsePathBase(appSetting.PathBase);

			app.UseForwardedHeaders(new ForwardedHeadersOptions {
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});

			// Show this message even from production environment only during the initial development phase
			//if (env.IsDevelopment()) {
			app.UseDeveloperExceptionPage();
			//} else {
			//	app.UseExceptionHandler("/Home/Error");
			//}

			app.UseHsts();
			//app.UseHttpsRedirection();
			app.UseStaticFiles(new StaticFileOptions() {
				// Max-age is 30 days (in seconds)
				OnPrepareResponse = context => context.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + (30 * 24 * 60 * 60)
			});

			app.UseMvc(routes => {
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}");
			});

			AppDomain.CurrentDomain.SetData("WWWRootDirectory", System.IO.Path.Combine(env.ContentRootPath, "wwwroot"));
			AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Path.Combine(env.ContentRootPath, "App_Data"));
		}
	}
}
