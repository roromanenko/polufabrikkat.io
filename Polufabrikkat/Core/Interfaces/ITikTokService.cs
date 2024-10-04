using Polufabrikkat.Core.Models.TikTok;

namespace Polufabrikkat.Core.Interfaces
{
	public interface ITikTokService
	{
		Task<AuthTokenData> GetAuthToken(string code, string processTikTokCallbackUrl);
		string GetLoginUrl(string redirectUrl, string returnUrl, CallbackStrategy callbackStrategy);
		TikTokHandleCallback GetTikTokHandleCallback(string state);

		ITikTokAuthenticatedService WithAuthData(AuthTokenData authTokenData);
	}

	public interface ITikTokAuthenticatedService
	{
		Task<QueryCreatorInfo> GetQueryCreatorInfo();
		Task<UserInfo> GetUserInfo();
		Task<string> PostPhoto(PostPhotoRequest apiRequest);
	}
}
