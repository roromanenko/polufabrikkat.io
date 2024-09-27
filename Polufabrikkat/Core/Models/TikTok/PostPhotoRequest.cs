namespace Polufabrikkat.Core.Models.TikTok
{
	public class PostPhotoRequest
	{
		public PostInfo PostInfo { get; set; }
		public SourceInfo SourceInfo { get; set; }
		public string PostMode { get; set; }
		public string MediaType { get; set; }
	}
	public class PostInfo
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public bool DisableComment { get; set; }
		public string PrivacyLevel { get; set; }
		public bool AutoAddMusic { get; set; }
	}

	public class SourceInfo
	{
		public string Source { get; set; }
		public int PhotoCoverIndex { get; set; }
		public List<string> PhotoImages { get; set; }
	}
}
