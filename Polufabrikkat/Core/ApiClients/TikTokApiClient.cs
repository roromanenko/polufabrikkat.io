using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models.TikTok;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

namespace Polufabrikkat.Core.ApiClients
{
	public class TikTokApiClient : ITikTokApiClient
	{
		private readonly HttpClient _httpClient;
		private readonly IMemoryCache _memoryCache;

		public TikTokApiClient(HttpClient httpClient, IMemoryCache memoryCache)
		{
			_httpClient = httpClient;
			_memoryCache = memoryCache;
		}

		public async Task<AuthTokenData> GetAuthToken(string decodedCode, string redirectUrl)
		{
			var accessTokenUrl = "https://open.tiktokapis.com/v2/oauth/token/";

			var clientKey = "***REMOVED***"; // from tiktok dev
			var clientSecret = "***REMOVED***";

			var queryString = new Dictionary<string, string>()
			{
				["client_key"] = clientKey,
				["client_secret"] = clientSecret,
				["code"] = decodedCode,
				["grant_type"] = "authorization_code",
				["redirect_uri"] = redirectUrl,
			};

			using var request = new HttpRequestMessage(HttpMethod.Post, accessTokenUrl)
			{
				Content = new FormUrlEncodedContent(queryString),
			};
			using var res = await _httpClient.SendAsync(request);

			var content = await res.Content.ReadFromJsonAsync<AuthTokenData>(new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
			});
			return content;
		}

		public string GetLoginUrl(string redirectUrl, string returnUrl, CallbackStrategy callbackStrategy)
		{
			var clientKey = "***REMOVED***"; // from tiktok dev
			var scope = "user.info.basic,video.publish,video.upload";
			var uniqueIdentificatorState = Guid.NewGuid().ToString("N");
			var responseType = "code";
			var queryString = new Dictionary<string, string>()
			{
				["client_key"] = clientKey,
				["scope"] = scope,
				["redirect_uri"] = redirectUrl,
				["state"] = uniqueIdentificatorState,
				["response_type"] = responseType
			};

			_memoryCache.Set(uniqueIdentificatorState, new TikTokHandleCallback
			{
				CallbackStrategy = callbackStrategy,
				ReturnUrl = returnUrl,
			}, TimeSpan.FromMinutes(2));

			var authorizationUrl = "https://www.tiktok.com/v2/auth/authorize/";
			var url = new Uri(QueryHelpers.AddQueryString(authorizationUrl, queryString));

			return url.ToString();
		}

		public async Task<UserInfo> GetUserInfo(AuthTokenData authData)
		{
			var getUserInfoUrl = "https://open.tiktokapis.com/v2/user/info/";
			var fields = "open_id,union_id,display_name";

			var url = new UriBuilder(getUserInfoUrl);
			url.Query = $"fields={fields}";

			using var request = new HttpRequestMessage(HttpMethod.Get, url.Uri);
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(authData.TokenType, authData.AccessToken);

			using var res = await _httpClient.SendAsync(request);
			var content = await res.Content.ReadFromJsonAsync<UserInfoResponse>(new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
			});

			return content.Data.User;
		}

		public async Task<QueryCreatorInfo> GetQueryCreatorInfo(AuthTokenData authData)
		{
			var url = "https://open.tiktokapis.com/v2/post/publish/creator_info/query/";

			using var request = new HttpRequestMessage(HttpMethod.Post, url);
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(authData.TokenType, authData.AccessToken);

			using var res = await _httpClient.SendAsync(request);
			var content = await res.Content.ReadFromJsonAsync<QueryCreatorInfoResponse>(new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
			});

			return content.Data;
		}
	}
}
