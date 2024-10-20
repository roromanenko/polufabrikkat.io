
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models.Entities;

namespace Polufabrikkat.Site.Jobs
{
	public class DelayedPublicationService : BackgroundService
	{
		private readonly ILogger<DelayedPublicationService> _logger;
		private readonly IServiceProvider _serviceProvider;

		private readonly TimeSpan _delay = TimeSpan.FromMinutes(5);

		public DelayedPublicationService(ILogger<DelayedPublicationService> logger, IServiceProvider serviceProvider)
		{
			_logger = logger;
			_serviceProvider = serviceProvider;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Background service is starting.");

			var timer = new PeriodicTimer(_delay);

			while (await timer.WaitForNextTickAsync(stoppingToken))
			{
				_logger.LogDebug($"Background service is running at (UTC): {DateTime.UtcNow}");

				using var scope = _serviceProvider.CreateScope();
				var postService = scope.ServiceProvider.GetRequiredService<IPostService>();
				var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
				var tikTokService = scope.ServiceProvider.GetRequiredService<ITikTokService>();

				var postsToPublish = await postService.GetFilteredPosts(
				statuses: [PostStatus.Created],
				scheduledPublicationTimeFrom: DateTime.UtcNow);

				foreach (var post in postsToPublish)
				{
					try
					{
						_logger.LogTrace($"Publish post ${post.Id}");
						var tikTokUser = await userService.GetTikTokUserByUnionId(post.TikTokUserUnionId);
						await tikTokService.WithAuthData(tikTokUser.AuthTokenData).PublishPhotoPost(post);
					}
					catch (Exception ex)
					{
						_logger.LogError($"Exception during post publishinh. Post id: {post.Id}. Exception: {ex}");
					}
				}
			}

			_logger.LogInformation("Background service is stopping.");
		}
	}
}
