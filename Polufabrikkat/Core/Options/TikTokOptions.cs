namespace Polufabrikkat.Core.Options
{
	public class TikTokOptions
	{
		public TimeSpan RefreshQueryCreatorInfoInterval { get; set; } = TimeSpan.FromHours(2);
		public TimeSpan LoginCacheInfoTime { get; set; } = TimeSpan.FromMinutes(2);
		public string RedirectToTikTokLoginUrl { get; set; }
	}
}
