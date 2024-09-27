namespace Polufabrikkat.Site.Models.User
{
	public class QueryCreatorInfoModel
	{
		public string CreatorNickname { get; set; }
		public string CreatorUsername { get; set; }
		public bool DuetDisabled { get; set; }
		public int MaxVideoPostDurationSec { get; set; }
		public List<string> PrivacyLevelOptions { get; set; }
		public bool StitchDisabled { get; set; }
		public bool CommentDisabled { get; set; }
		public string CreatorAvatarUrl { get; set; }
	}
}
