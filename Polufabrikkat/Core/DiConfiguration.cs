using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Polufabrikkat.Core.ApiClients;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Options;
using Polufabrikkat.Core.Repositories;

namespace Polufabrikkat.Core
{
	public static class DiConfiguration
	{
		public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped(opt =>
			{
				var settings = MongoClientSettings.FromConnectionString(configuration.GetConnectionString("MongoDb"));
				settings.ServerApi = new ServerApi(ServerApiVersion.V1);
				var client = new MongoClient(settings);

				return client;
			});

			services.Configure<MongoDbOptions>(configuration.GetSection(nameof(MongoDbOptions)));
			services.Configure<TikTokOptions>(configuration.GetSection(nameof(TikTokOptions)));
			services.AddScoped<ITikTokApiClient, TikTokApiClient>();
			services.AddScoped<IUserRepository, UserRepository>();

			return services;
		}
	}
}
