using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using MongoDB.Driver;
using Polufabrikkat.Core.Options;
using Polufabrikkat.Site.Options;
using SixLabors.ImageSharp;

namespace Polufabrikkat.Site
{
	public static class DiConfiguration
	{
		private const string SpecificOrigins = "_myAllowSpecificOrigins";

		public static IServiceCollection AddSiteServices(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
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

			services.Configure<MongoDbOptions>(configuration.GetSection(nameof(MongoDbOptions)));
			services.Configure<TikTokOptions>(configuration.GetSection(nameof(TikTokOptions)));
			services.Configure<TikTokApiOptions>(config =>
			{
				config = configuration.GetSection(nameof(TikTokApiOptions)).Get<TikTokApiOptions>();
				if (environment.IsDevelopment())
				{
					config.ClientKey = configuration["Polufabrikkat:TikTokClientKey"];
					config.ClientSecret = configuration["Polufabrikkat:TikTokClientSecret"];
				}
				else
				{
					config.ClientKey = Environment.GetEnvironmentVariable("TIKTOK_CLIENT_KEY");
					config.ClientSecret = Environment.GetEnvironmentVariable("TIKTOK_CLIENT_SECRET");
				}
			});
			services.AddScoped(opt =>
			{
				string connectionString;
				if (environment.IsDevelopment())
				{
					connectionString = configuration["Polufabrikkat:MongodbConnection"];
				}
				else
				{
					connectionString = Environment.GetEnvironmentVariable("MONGODB_STRING");
				}

				var settings = MongoClientSettings.FromConnectionString(connectionString);
				settings.ServerApi = new ServerApi(ServerApiVersion.V1);
				var client = new MongoClient(settings);

				return client;
			});

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
			app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			return app;
		}
	}
}
