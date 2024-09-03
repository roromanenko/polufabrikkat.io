using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Site.Models;
using Polufabrikkat.Site.Options;
using System.Diagnostics;
using System.Web;

namespace Polufabrikkat.Site.Controllers
{
	public partial class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ITikTokApiClient _tikTokApiClient;
		private readonly FileUploadOptions _fileUploadOptions;

		public HomeController(ILogger<HomeController> logger, IOptions<FileUploadOptions> fileUploadOptions, ITikTokApiClient tikTokApiClient)
		{
			_logger = logger;
			_tikTokApiClient = tikTokApiClient;
			_fileUploadOptions = fileUploadOptions.Value;
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
			return RedirectToAction("Index", "Home");
		}
	}
}