using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Polufabrikkat.Core.ApiClients;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Options;
using Polufabrikkat.Core.Repositories;
using Polufabrikkat.Core.Services;

namespace Polufabrikkat.Core
{
	public static class DiConfiguration
	{
		public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<ITikTokApiClient, TikTokApiClient>();
			services.AddScoped<ITikTokService, TikTokService>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IFileRepository, FileRepository>();
			services.AddScoped<IPostRepository, PostRepository>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IPostService, PostService>();

			return services;
		}
	}
}
