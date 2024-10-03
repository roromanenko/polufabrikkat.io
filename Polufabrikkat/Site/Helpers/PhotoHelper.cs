namespace Polufabrikkat.Site.Helpers
{
	public static class PhotoHelper
	{
		private static readonly Dictionary<string, string> AllowedPhotoMimeTypeExtensions = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "image/jpeg", ".jpg" },
			{ "image/webp", ".webp" },
		};

		public static bool IsMimeTypeAllowed(string mimeType) => AllowedPhotoMimeTypeExtensions.ContainsKey(mimeType);

		public static string GetExtensionFromMimeType(string mimeType)
			=> AllowedPhotoMimeTypeExtensions.TryGetValue(mimeType, out var extension) ? extension : default;
	}
}
