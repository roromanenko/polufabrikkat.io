using Polufabrikkat.Core.Models.TikTok;

namespace Polufabrikkat.Site.Models.User
{
	public class UserModel
	{
		public string Id { get; set; }
		public string Username { get; set; }
		public List<TikTokUserModel> TikTokUsers { get; set; } = new List<TikTokUserModel>();
	}
}
