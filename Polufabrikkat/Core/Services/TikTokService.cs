using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models.TikTok;
using Polufabrikkat.Core.Options;

namespace Polufabrikkat.Core.Services
{
	public class TikTokService : ITikTokService
	{
		private readonly ITikTokApiClient _tikTokApiClient;
		private readonly IMemoryCache _memoryCache;
		private readonly IUserRepository _userRepository;
		private readonly TikTokOptions _tikTokOptions;

		public TikTokService(ITikTokApiClient tikTokApiClient, IMemoryCache memoryCache, IUserRepository userRepository, IOptions<TikTokOptions> tikTokOptions)
		{
			_tikTokApiClient = tikTokApiClient;
			_memoryCache = memoryCache;
			_userRepository = userRepository;
			_tikTokOptions = tikTokOptions.Value;
		}
		public Task<AuthTokenData> GetAuthToken(string code, string processTikTokCallbackUrl)
		{
			return _tikTokApiClient.GetAuthToken(code, processTikTokCallbackUrl);
		}

		public string GetLoginUrl(string redirectUrl, string returnUrl, CallbackStrategy callbackStrategy)
		{
			return _tikTokApiClient.GetLoginUrl(redirectUrl, returnUrl, callbackStrategy);
		}

		public TikTokHandleCallback GetTikTokHandleCallback(string state)
		{
			TikTokHandleCallback tikTokHandleCallback = null;
			if (!string.IsNullOrEmpty(state))
			{
				if (_memoryCache.TryGetValue(state, out tikTokHandleCallback))
				{
					_memoryCache.Remove(state);
				}
			}

			return tikTokHandleCallback;
		}

		public ITikTokAuthenticatedService WithAuthData(AuthTokenData authTokenData)
		{
			return new TikTokAuthenticatedService(_tikTokApiClient, _userRepository, _tikTokOptions, authTokenData);
		}
	}

	public class TikTokAuthenticatedService : ITikTokAuthenticatedService
	{
		private readonly ITikTokApiClient _tikTokApiClient;
		private AuthTokenData _authTokenData;
		private readonly IUserRepository _userRepository;
		private readonly TikTokOptions _tikTokOptions;

		public TikTokAuthenticatedService(ITikTokApiClient tikTokApiClient, IUserRepository userRepository,
			TikTokOptions tikTokOptions, AuthTokenData authTokenData)
		{
			_tikTokApiClient = tikTokApiClient;
			_authTokenData = authTokenData;
			_userRepository = userRepository;
			_tikTokOptions = tikTokOptions;
		}

		public async Task<UserInfo> GetUserInfo()
		{
			await VerifyTokenDataAndRefreshIfNeeded();
			return await _tikTokApiClient.GetUserInfo(_authTokenData);
		}

		public async Task<string> PostPhoto(PostPhotoRequest apiRequest)
		{
			await VerifyTokenDataAndRefreshIfNeeded();
			return await _tikTokApiClient.PostPhoto(_authTokenData, apiRequest);
		}

		public async Task<QueryCreatorInfo> GetQueryCreatorInfo()
		{
			var queryCreatorInfo = await _userRepository.GetQueryCreatorInfoByOpenId(_authTokenData.OpenId);
			if (queryCreatorInfo != null
				&& ((DateTime.UtcNow - queryCreatorInfo.RefreshedDateTime) < _tikTokOptions.RefreshQueryCreatorInfoInterval))
			{
				return queryCreatorInfo;
			}

			await VerifyTokenDataAndRefreshIfNeeded();
			queryCreatorInfo = await _tikTokApiClient.GetQueryCreatorInfo(_authTokenData);
			await _userRepository.UpdateQueryCreatorInfo(_authTokenData.OpenId, queryCreatorInfo);

			return queryCreatorInfo;
		}

		private async Task VerifyTokenDataAndRefreshIfNeeded()
		{
			if ((DateTime.UtcNow - _authTokenData.RefreshedDate) >= TimeSpan.FromSeconds(_authTokenData.ExpiresIn))
			{
				_authTokenData = await RefreshTokenData(_authTokenData);
				await _userRepository.UpdateAuthData(_authTokenData);
			}
		}

		private Task<AuthTokenData> RefreshTokenData(AuthTokenData authTokenData)
		{
			return _tikTokApiClient.RefreshTokenData(authTokenData);
		}
	}
}
