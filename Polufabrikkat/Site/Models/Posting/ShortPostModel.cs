using Polufabrikkat.Core.Models.Entities;
using Polufabrikkat.Site.Interfaces;

namespace Polufabrikkat.Site.Models.Posting
{
	public class ShortPostModel
	{
		public string Id { get; set; }
		public string TikTokUserUnionId { get; set; }
		public string Type { get; set; }
		public string Status { get; set; }
		public DateTime Created { get; set; }
		public DateTime? ScheduledPublicationTime { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public int FilesCount { get; set; }

		public ShortPostModel()
		{
		}

		public ShortPostModel(Post post, IDateTimeProvider dateTimeProvider)
		{
			Id = post.Id.ToString();
			TikTokUserUnionId = post.TikTokUserUnionId;
			Type = post.Type.ToString();
			Status = post.Status.ToString();
			Created = dateTimeProvider.ConvertToClienTimezoneFromUtc(post.Created);
			ScheduledPublicationTime = dateTimeProvider.ConvertToClienTimezoneFromUtc(post.ScheduledPublicationTime);
			Title = post.TikTokPostInfo.Title;
			Description = post.TikTokPostInfo.Description;
			FilesCount = post.FileIds.Count;
		}
	}
}
