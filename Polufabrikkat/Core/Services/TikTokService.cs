using Microsoft.Extensions.Caching.Memory;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models.TikTok;

namespace Polufabrikkat.Core.Services
{
	public class TikTokService : ITikTokService
	{
		private readonly ITikTokApiClient _tikTokApiClient;
		private readonly IMemoryCache _memoryCache;
		private readonly IUserRepository _userRepository;

		public TikTokService(ITikTokApiClient tikTokApiClient, IMemoryCache memoryCache, IUserRepository userRepository)
		{
			_tikTokApiClient = tikTokApiClient;
			_memoryCache = memoryCache;
			_userRepository = userRepository;
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
			return new TikTokAuthenticatedService(_tikTokApiClient, _userRepository, authTokenData);
		}
	}

	public class TikTokAuthenticatedService : ITikTokAuthenticatedService
	{
		private readonly ITikTokApiClient _tikTokApiClient;
		private AuthTokenData _authTokenData;
		private readonly IUserRepository _userRepository;

		public TikTokAuthenticatedService(ITikTokApiClient tikTokApiClient, IUserRepository userRepository, AuthTokenData authTokenData)
		{
			_tikTokApiClient = tikTokApiClient;
			_authTokenData = authTokenData;
			_userRepository = userRepository;
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
			await VerifyTokenDataAndRefreshIfNeeded();
			return await _tikTokApiClient.GetQueryCreatorInfo(_authTokenData);
		}

		private async Task VerifyTokenDataAndRefreshIfNeeded()
		{
			if ((DateTime.UtcNow - _authTokenData.RefreshedDate) > TimeSpan.FromSeconds(_authTokenData.ExpiresIn))
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
