using Polufabrikkat.Core.Models.TikTok;

namespace Polufabrikkat.Core.Interfaces
{
	public interface ITikTokApiClient
	{
		Task<AuthTokenData> GetAuthToken(string decodedCode, string redirectUrl);
		string GetLoginUrl(string redirectUrl, string uniqueIdentificator);
		Task<UserInfo> GetUserInfo(AuthTokenData authData);
		Task<QueryCreatorInfo> GetQueryCreatorInfo(AuthTokenData authData);
		Task<AuthTokenData> RefreshTokenData(AuthTokenData authTokenData);
		Task<string> PublishPhotoPost(AuthTokenData authData, PostPhotoRequest postRequest);
		Task<PostStatusData> GetPostStatus(AuthTokenData authData, PostStatusRequest request);
	}
}
