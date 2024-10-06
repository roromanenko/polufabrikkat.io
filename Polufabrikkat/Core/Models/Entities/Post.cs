using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Polufabrikkat.Core.Models.TikTok;
using System.ComponentModel.DataAnnotations.Schema;

namespace Polufabrikkat.Core.Models.Entities
{
	[Table("posts")]
	public class Post
	{
		[BsonId]
		public ObjectId Id { get; set; }
		public ObjectId UserId { get; set; }
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
		public PostStatusData TikTokPostStatus { get; set; }

	}
	public class TikTokPostInfo
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string PrivacyLevel { get; set; }
		public bool DisableComment { get; set; }
		public bool AutoAddMusic { get; set; }
		public int PhotoCoverIndex { get; set; }
	}

	public enum PostType
	{
		Photo = 0,
		Video = 1
	}

	public enum PostStatus
	{
		Created = 0,
		/// <summary>
		/// Post sent to TikTok, <see cref="Post.TikTokPublishId"/> should be filled
		/// </summary>
		SentToTikTok = 1,
		Failed = 2,
		Published = 3
	}
}
