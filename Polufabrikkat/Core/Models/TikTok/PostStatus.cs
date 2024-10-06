namespace Polufabrikkat.Core.Models.TikTok
{
	public class PostStatusRequest
	{
		public string PublishId { get; set; }
	}

	public class PostStatusResponse
	{
		public PostStatusData Data { get; set; }
		public PostStatusError Error { get; set; }
	}

	public class PostStatusData
	{
		public string Status { get; set; }
		public string FailReason { get; set; }
		public List<long> PublicalyAvailablePostId { get; set; }
		public int UploadedBytes { get; set; }
	}

	public class PostStatusError
	{
		public string Code { get; set; }
		public string Message { get; set; }
		public string LogId { get; set; }
	}
}
