namespace Polufabrikkat.Site.Models.User
{
	public class UserViewModel : BaseModel
	{
		public UserModel User { get; set; }
		public List<PostShortInto> Posts { get; set; }
		public string AddTikTokUserUrl { get; set; }
	}
}
