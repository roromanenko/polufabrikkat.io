using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Polufabrikkat.Core.ApiClients;
using Polufabrikkat.Core.Interfaces;

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

			services.AddScoped<ITikTokApiClient, TikTokApiClient>();

			return services;
		}
	}
}
