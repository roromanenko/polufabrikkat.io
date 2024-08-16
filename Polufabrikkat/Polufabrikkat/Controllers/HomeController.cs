using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Polufabrikkat.Models;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Web;
using static System.Formats.Asn1.AsnWriter;

namespace Polufabrikkat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly HttpClient _httpClient;

		public HomeController(ILogger<HomeController> logger, HttpClient httpClient)
        {
            _logger = logger;
			_httpClient = httpClient;
		}

        public async Task<IActionResult> Index()
        {
			return View();
        }

		public IActionResult Privacy()
        {
            return View();
        }

		public IActionResult TermsOfService()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

		public IActionResult RedirectToTikTokLogin()
		{
			var clientKey = "***REMOVED***"; // from tiktok dev
			var scope = "user.info.basic,video.publish,video.upload";
			var redirectUrl = Url.Action("ProcessTikTokLoginResponse", "Home", null, Request.Scheme, Request.Host.Value);
			var uniqueIdentificatorState = Guid.NewGuid().ToString("N");
			var responseType = "code";
			var queryString = new Dictionary<string, string>()
			{
				["client_key"] = clientKey,
				["scope"] = scope,
				["redirect_uri"] = redirectUrl,
				["state"] = uniqueIdentificatorState,
				//["code_challenge"] = uniqueIdentificatorState,
				//["code_challenge_method"] = "plain",
				["response_type"] = responseType
			};

			var authorizationUrl = "https://www.tiktok.com/v2/auth/authorize/";
			var url = new Uri(QueryHelpers.AddQueryString(authorizationUrl, queryString));

			return Redirect(url.ToString());
		}

		public async Task<IActionResult> ProcessTikTokLoginResponse()
		{
			if (!Request.Query.ContainsKey("access_token"))
			{
				var response = new
				{
					Code = Request.Query["code"],
					Scopes = Request.Query["scopes"],
					State = Request.Query["state"],
					Error = Request.Query["error"],
					Error_description = Request.Query["error_description"],
				};

				var clientKey = "***REMOVED***"; // from tiktok dev
				var clientSecret = "***REMOVED***";
				var redirectUrl = Url.Action("ProcessTikTokLoginResponse", "Home", null, Request.Scheme, Request.Host.Value);

				var accessTokenUrl = "https://open.tiktokapis.com/v2/oauth/token/";
				var queryString = new Dictionary<string, string>()
				{
					["client_key"] = clientKey,
					["client_secret"] = clientSecret,
					["code"] = HttpUtility.UrlDecode(response.Code),
					["grant_type"] = "authorization_code",
					["redirect_uri"] = redirectUrl,
				};

				using var request = new HttpRequestMessage(HttpMethod.Post, accessTokenUrl)
				{
					Content = new FormUrlEncodedContent(queryString),
				};
				//request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
				using var res = await _httpClient.SendAsync(request);

				var content = await res.Content.ReadAsStringAsync();

			}
			var acctresponse = Request;
			return RedirectToAction("Index", "Home");
		}
	}
}