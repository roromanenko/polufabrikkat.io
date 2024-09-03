using Microsoft.Extensions.DependencyInjection;
using Polufabrikkat.Core.ApiClients;
using Polufabrikkat.Core.Interfaces;

namespace Polufabrikkat.Core
{
	public static class DiConfiguration
	{
		public static IServiceCollection AddCoreServices(this IServiceCollection services)
		{
			services.AddScoped<ITikTokApiClient, TikTokApiClient>();

			return services;
		}
	}
}
