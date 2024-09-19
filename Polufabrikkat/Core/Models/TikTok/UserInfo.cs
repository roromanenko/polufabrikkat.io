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
		/// <summary>
		/// The unique identification of the user across different apps for the same developer.
		/// </summary>
		public string UnionId { get; set; }
		/// <summary>
		/// User's profile name
		/// </summary>
		public string DisplayName { get; set; }
		/// <summary>
		/// The unique identification of the user in the current application.
		/// </summary>
		public string OpenId { get; set; }
	}

	public class UserInfoError
	{
		public string Code { get; set; }
		public string Message { get; set; }
		public string LogId { get; set; }
	}
}
