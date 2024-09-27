namespace Polufabrikkat.Site.Models.Posting
{
	public class NewPhotoPostRequest
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string PrivacyLevel { get; set; }
		public bool DisableComment { get; set; }
		public bool AutoAddMusic { get; set; }
		public int PhotoCoverIndex { get; set; }

		public List<IFormFile> Files { get; set; }
	}
}
