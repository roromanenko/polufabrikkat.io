using Polufabrikkat.Core.Models.TikTok;

namespace Polufabrikkat.Core.Interfaces
{
	public interface ITikTokApiClient
	{
		public Task<AuthTokenData> GetAuthToken(string decodedCode, string redirectUrl);
		public string GetLoginUrl(string redirectUrl, string returnUrl, CallbackStrategy callbackStrategy);
		public Task<UserInfo> GetUserInfo(AuthTokenData authData);
	}
}
