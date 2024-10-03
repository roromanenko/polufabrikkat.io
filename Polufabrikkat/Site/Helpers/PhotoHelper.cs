using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace Polufabrikkat.Site.Helpers
{
	public static class PhotoHelper
	{
		const long MaxFileSize = 10 * 1024 * 1024;

		private static readonly Dictionary<string, string> AllowedPhotoMimeTypeExtensions = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "image/jpeg", ".jpg" },
			{ "image/webp", ".webp" },
		};

		public static bool IsMimeTypeAllowed(string mimeType) => AllowedPhotoMimeTypeExtensions.ContainsKey(mimeType);

		public static bool IsValidFileSize(long size) => size < MaxFileSize;

		/// <summary>
		/// Picture size restrictions: Maximum 1080p 
		/// https://developers.tiktok.com/doc/content-posting-api-media-transfer-guide#
		/// </summary>
		public static bool IsValidPictureSize(Stream stream)
		{
			try
			{
				var img = Image.Load<Rgba32>(stream);

				int width = img.Width;
				int height = img.Height;

				return width <= 1080 && height <= 1080;
			}
			catch
			{
				return false;
			}
		}

		public static string GetExtensionFromMimeType(string mimeType)
			=> AllowedPhotoMimeTypeExtensions.TryGetValue(mimeType, out var extension) ? extension : default;
 
	}
}
