namespace Polufabrikkat.Core.Models.TikTok
{
	public class UserInfoResponse
	{
		public UserInfoData Data { get; set; }
		public UserInfoError Error { get; set; }
	}

	public class UserInfoData
	{
		public UserInfo User { get; set; }
	}

	public class UserInfo
	{
		public string UnionId { get; set; }
		public string DisplayName { get; set; }
		public string OpenId { get; set; }
	}

	public class UserInfoError
	{
		public string Code { get; set; }
		public string Message { get; set; }
		public string LogId { get; set; }
	}
}
