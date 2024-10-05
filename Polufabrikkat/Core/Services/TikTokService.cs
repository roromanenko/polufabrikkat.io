using Amazon.Runtime.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models.Entities;
using Polufabrikkat.Core.Models.TikTok;
using Polufabrikkat.Core.Options;

namespace Polufabrikkat.Core.Services
{
	public class TikTokService : ITikTokService
	{
		private readonly ITikTokApiClient _tikTokApiClient;
		private readonly IMemoryCache _memoryCache;
		private readonly IUserRepository _userRepository;
		private readonly IFileRepository _fileRepository;
		private readonly IPostRepository _postRepository;
		private readonly TikTokOptions _tikTokOptions;

		public TikTokService(ITikTokApiClient tikTokApiClient, IMemoryCache memoryCache,
			IUserRepository userRepository, IFileRepository fileRepository, IPostRepository postRepository,
			IOptions<TikTokOptions> tikTokOptions)
		{
			_tikTokApiClient = tikTokApiClient;
			_memoryCache = memoryCache;
			_userRepository = userRepository;
			_fileRepository = fileRepository;
			_postRepository = postRepository;
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

		public async Task<Post> AddNewPost(Post post, List<Models.Entities.File> files)
		{
			var fileIds = new List<ObjectId>();
			foreach (var file in files)
			{
				var addedFile = await _fileRepository.SaveFile(file);
				fileIds.Add(addedFile.Id);
			}
			post.FileIds = fileIds;
			post = await _postRepository.AddPost(post);

			return post;
		}

		public ITikTokAuthenticatedService WithAuthData(AuthTokenData authTokenData)
		{
			return new TikTokAuthenticatedService(_tikTokApiClient, _userRepository,
				_postRepository, _tikTokOptions, authTokenData);
		}
	}

	public class TikTokAuthenticatedService : ITikTokAuthenticatedService
	{
		private readonly ITikTokApiClient _tikTokApiClient;
		private AuthTokenData _authTokenData;
		private readonly IUserRepository _userRepository;
		private readonly IPostRepository _postRepository;
		private readonly TikTokOptions _tikTokOptions;

		public TikTokAuthenticatedService(ITikTokApiClient tikTokApiClient, IUserRepository userRepository,
			IPostRepository postRepository,
			TikTokOptions tikTokOptions, AuthTokenData authTokenData)
		{
			_tikTokApiClient = tikTokApiClient;
			_authTokenData = authTokenData;
			_userRepository = userRepository;
			_postRepository = postRepository;
			_tikTokOptions = tikTokOptions;
		}

		public async Task<UserInfo> GetUserInfo()
		{
			await VerifyTokenDataAndRefreshIfNeeded();
			return await _tikTokApiClient.GetUserInfo(_authTokenData);
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

		public async Task PublishPhotoPost(Post post)
		{
			await VerifyTokenDataAndRefreshIfNeeded();
			var apiRequest = new PostPhotoRequest
			{
				PostInfo = new PostInfo
				{
					Title = post.TikTokPostInfo.Title,
					Description = post.TikTokPostInfo.Description,
					PrivacyLevel = post.TikTokPostInfo.PrivacyLevel,
					DisableComment = post.TikTokPostInfo.DisableComment,
					AutoAddMusic = post.TikTokPostInfo.AutoAddMusic,
				},
				SourceInfo = new SourceInfo
				{
					PhotoCoverIndex = post.TikTokPostInfo.PhotoCoverIndex,
					PhotoImages = post.FileUrls
				}
			};
			var publishId = await _tikTokApiClient.PublishPhotoPost(_authTokenData, apiRequest);
			await _postRepository.MarkAsSentToTikTok(post.Id, publishId);
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
