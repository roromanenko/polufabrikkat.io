namespace Polufabrikkat.Core.Models.TikTok
{
	public class PostPhotoResponse
	{
		public PostPhotoResponseData Data { get; set; }
		public PostPhotoResponseError Error { get; set; }
	}

	public class PostPhotoResponseData
	{
		public string PublishId { get; set; }
	}

	public class PostPhotoResponseError
	{
		public string Code { get; set; }
		public string Message { get; set; }
		public string LogId { get; set; }
	}
}
