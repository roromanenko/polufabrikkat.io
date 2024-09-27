namespace Polufabrikkat.Core.Models.TikTok
{
	public class AuthTokenData
	{
		public string AccessToken { get; set; }
		public int ExpiresIn { get; set; }
		public string OpenId { get; set; }
		public int RefreshExpiresIn { get; set; }
		public string RefreshToken { get; set; }
		public string Scope { get; set; }
		public string TokenType { get; set; } = "Bearer";
		public DateTime RefreshedDate { get; set; } = DateTime.UtcNow;
    }
}
