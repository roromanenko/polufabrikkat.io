namespace Polufabrikkat.Site.Models.Posting
{
	public class RefreshTikTokPublicationStatusRequest
	{
		public string PostId { get; set; }
		public string PublicationId { get; set; }
		public string TikTokUserUnionId { get; set; }
	}
}
