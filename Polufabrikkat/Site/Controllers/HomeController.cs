using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Site.Models;
using System.Diagnostics;
using System.Security.Claims;
using System.Web;

namespace Polufabrikkat.Site.Controllers
{
	[AllowAnonymous]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ITikTokApiClient _tikTokApiClient;

		public HomeController(ILogger<HomeController> logger, ITikTokApiClient tikTokApiClient)
		{
			_logger = logger;
			_tikTokApiClient = tikTokApiClient;
		}

		public IActionResult Index()
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
			Request.IsHttps = true;
			var redirectUrl = Url.Action("ProcessTikTokLoginResponse", "Home", null, Request.Scheme, Request.Host.Value);
			var loginUrl = _tikTokApiClient.GetLoginUrl(redirectUrl);
			return Redirect(loginUrl);
		}

		public async Task<IActionResult> ProcessTikTokLoginResponse()
		{
			var response = new
			{
				Code = Request.Query["code"],
				Scopes = Request.Query["scopes"],
				State = Request.Query["state"],
				Error = Request.Query["error"],
				Error_description = Request.Query["error_description"],
			};

			var redirectUrl = Url.Action("ProcessTikTokLoginResponse", "Home", null, Request.Scheme, Request.Host.Value);
			var tokenData = await _tikTokApiClient.GetAuthToken(HttpUtility.UrlDecode(response.Code), redirectUrl);
			var userInfo = await _tikTokApiClient.GetUserInfo(tokenData);

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, userInfo.DisplayName)
			};
			var claimsIdentity = new ClaimsIdentity(claims, "Login");
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

			return RedirectToAction("Index", "Posting");
		}
	}
}