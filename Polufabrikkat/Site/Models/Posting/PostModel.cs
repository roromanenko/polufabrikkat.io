using MongoDB.Bson;
using Polufabrikkat.Core.Models.Entities;

namespace Polufabrikkat.Site.Models.Posting
{
	public class PostModel
	{
		public string Id { get; set; }
		public string UserId { get; set; }
		public string TikTokUserUnionId { get; set; }
		public TikTokPostInfo TikTokPostInfo { get; set; }
		public PostType Type { get; set; }
		public PostStatus Status { get; set; }
		public DateTime Created { get; set; }
		/// <summary>
		/// If this field is null, post should be sent to tiktok immediately.
		/// In other case this is the date when post will be sent to tiktok
		/// </summary>
		public DateTime? ScheduledPublicationTime { get; set; }
		public List<string> FileUrls { get; set; }
		public List<ObjectId> FileIds { get; set; }
		public string TikTokPublishId { get; set; }
	}
	public class TikTokPostInfoModel
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string PrivacyLevel { get; set; }
		public bool DisableComment { get; set; }
		public bool AutoAddMusic { get; set; }
		public int PhotoCoverIndex { get; set; }
	}
}
