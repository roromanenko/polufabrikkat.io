using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using Polufabrikkat.Site.Interfaces;
using Polufabrikkat.Site.Options;
using Polufabrikkat.Site.Services;

namespace Polufabrikkat.Site
{
	public static class DiConfiguration
	{
		private const string SpecificOrigins = "_myAllowSpecificOrigins";

		public static IServiceCollection AddSiteServices(this IServiceCollection services, IWebHostEnvironment environment)
		{
			services.AddControllersWithViews();
			services.AddCors(options =>
			{
				options.AddPolicy(name: SpecificOrigins,
					policy =>
					{
						policy.AllowAnyOrigin()
							.AllowAnyMethod()
							.AllowAnyHeader();
					});
			});
			services.AddHttpClient();
			services.AddMemoryCache();
			services.AddAutoMapper(typeof(SiteMapperProfile));

			services.Configure<FileUploadOptions>(x =>
			{
				x.FileUploadPath = Path.Combine(environment.ContentRootPath, "Images");
			});

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.LoginPath = "/Home/Login";
					options.LogoutPath = "/Home/Logout";
				});

			services.AddScoped<IUserService, UserService>();

			return services;
		}

		public static WebApplication UseSiteServices(this WebApplication app, IWebHostEnvironment environment)
		{
			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			var imagesPath = Path.Combine(environment.ContentRootPath, "Images");
			if (!Directory.Exists(imagesPath))
			{
				Directory.CreateDirectory(imagesPath);
			}
			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(imagesPath),
				RequestPath = "/Images"
			});

			app.UseRouting();
			app.UseCors(SpecificOrigins);

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			return app;
		}
	}
}
