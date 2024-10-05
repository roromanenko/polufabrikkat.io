using Polufabrikkat.Core.Models.Entities;
using Polufabrikkat.Core.Models.TikTok;

namespace Polufabrikkat.Core.Interfaces
{
	public interface ITikTokService
	{
		Task<AuthTokenData> GetAuthToken(string code, string processTikTokCallbackUrl);
		string GetLoginUrl(string redirectUrl, string returnUrl, CallbackStrategy callbackStrategy);
		TikTokHandleCallback GetTikTokHandleCallback(string state);
		Task<Post> AddNewPost(Post post, List<Models.Entities.File> files);

		ITikTokAuthenticatedService WithAuthData(AuthTokenData authTokenData);
	}

	public interface ITikTokAuthenticatedService
	{
		Task<QueryCreatorInfo> GetQueryCreatorInfo();
		Task<UserInfo> GetUserInfo();
		Task PublishPhotoPost(Post newPost);
	}
}
