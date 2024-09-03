using Microsoft.AspNetCore.WebUtilities;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models.TikTok;
using System.Net.Http.Json;
using System.Web;

namespace Polufabrikkat.Core.ApiClients
{
	public class TikTokApiClient : ITikTokApiClient
	{
		private readonly HttpClient _httpClient;

		public TikTokApiClient(HttpClient httpClient)
		{
			_httpClient = httpClient;
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

			var content = await res.Content.ReadFromJsonAsync<AuthTokenData>();
			return content;
		}

		public string GetLoginUrl(string redirectUrl)
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

			var authorizationUrl = "https://www.tiktok.com/v2/auth/authorize/";
			var url = new Uri(QueryHelpers.AddQueryString(authorizationUrl, queryString));

			return url.ToString();
		}
	}
}
