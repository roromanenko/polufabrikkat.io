namespace Polufabrikkat.Core.Models.TikTok
{
	public class QueryCreatorInfoResponse
	{
		public QueryCreatorInfo Data { get; set; }
		public QueryCreatorInfoError Error { get; set; }
	}

	public class QueryCreatorInfo
	{
		public string CreatorNickname { get; set; }
		public string CreatorUsername { get; set; }
		public bool DuetDisabled { get; set; }
		public int MaxVideoPostDurationSec { get; set; }
		public List<string> PrivacyLevelOptions { get; set; }
		public bool StitchDisabled { get; set; }
		public bool CommentDisabled { get; set; }
		public string CreatorAvatarUrl { get; set; }
		public DateTime RefreshedDateTime { get; set; }
	}

	public class QueryCreatorInfoError
	{
		public string Code { get; set; }
		public string Message { get; set; }
		public string LogId { get; set; }
	}
}
