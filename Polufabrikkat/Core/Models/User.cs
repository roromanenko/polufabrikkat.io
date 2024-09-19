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
		public TikTokUser TikTokUser { get; set; }
	}
}
