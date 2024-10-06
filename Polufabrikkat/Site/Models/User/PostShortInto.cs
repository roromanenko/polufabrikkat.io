using Polufabrikkat.Core.Models.Entities;

namespace Polufabrikkat.Site.Models.User
{
	public class PostShortInto
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime Created { get; set; }
		public PostStatus Status { get; set; }
		public string TikTokPublishId { get; set; }
	}
}
