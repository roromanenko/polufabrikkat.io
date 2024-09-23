using Polufabrikkat.Core.Models.TikTok;
using System.ComponentModel.DataAnnotations.Schema;

namespace Polufabrikkat.Core.Models
{
	[Table("users")]
	public class User
	{
		public string Id { get; set; }
		public string Username { get; set; }
		public string PasswordHash { get; set; }
		public List<string> Roles { get; set; } = new List<string>();
		public List<TikTokUser> TikTokUsers { get; set; } = new List<TikTokUser>();
	}
}
