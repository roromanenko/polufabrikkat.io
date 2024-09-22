using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models;
using Polufabrikkat.Core.Models.TikTok;
using Polufabrikkat.Site.Interfaces;
using Polufabrikkat.Site.Models;
using System.Diagnostics;
using System.Web;

namespace Polufabrikkat.Site.Controllers
{
	[AllowAnonymous]
	public class HomeController : BaseController
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ITikTokApiClient _tikTokApiClient;
		private readonly IMemoryCache _memoryCache;
		private readonly IUserService _userService;

		public HomeController(ILogger<HomeController> logger, ITikTokApiClient tikTokApiClient, IMemoryCache memoryCache, IUserService userService)
		{
			_logger = logger;
			_tikTokApiClient = tikTokApiClient;
			_memoryCache = memoryCache;
			_userService = userService;
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

		public IActionResult Login(string returnUrl = null)
		{
			var model = new LoginModel
			{
				ReturnUrl = returnUrl
			};
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginModel model)
		{
			var user = await _userService.VerifyUserLogin(model);
			if (user != null)
			{
				await LoginUser(user);
				if (!string.IsNullOrEmpty(model.ReturnUrl))
				{
					return Redirect(model.ReturnUrl);
				}
				return RedirectToAction("Index", "Posting");
			}
			model.Error = "Incorrect Username or/and Password";
			return View(model);
		}


		[HttpPost]
		public async Task<IActionResult> Register(LoginModel model)
		{
			try
			{
				User user = await _userService.RegisterUser(model);
				await LoginUser(user);
				if (!string.IsNullOrEmpty(model.ReturnUrl))
				{
					return Redirect(model.ReturnUrl);
				}
				return RedirectToAction("Index", "Posting");
			}
			catch (ArgumentException ex)
			{
				model.Error = ex.Message;
				return View(nameof(Login), model);
			}
		}

		public async Task<IActionResult> Logout()
		{
			await LogoutUser();
			return RedirectToAction("Index", "Home");
		}

		public IActionResult RedirectToTikTokLogin([FromQuery] string returnUrl, [FromQuery] CallbackStrategy? callbackStrategy)
		{
			Request.IsHttps = true;
			var redirectUrl = Url.Action("ProcessTikTokLoginResponse", "Home", null, Request.Scheme, Request.Host.Value);
			var loginUrl = _tikTokApiClient.GetLoginUrl(redirectUrl, returnUrl, callbackStrategy ?? CallbackStrategy.Login);
			return Redirect(loginUrl);
		}

		public async Task<IActionResult> ProcessTikTokLoginResponse()
		{
			Request.IsHttps = true;
			var response = new TikTokCallbackResponse(Request.Query);

			var redirectUrl = Url.Action("ProcessTikTokLoginResponse", "Home", null, Request.Scheme, Request.Host.Value);
			var tokenData = await _tikTokApiClient.GetAuthToken(HttpUtility.UrlDecode(response.Code), redirectUrl);
			var userInfo = await _tikTokApiClient.GetUserInfo(tokenData);

			TikTokHandleCallback tikTokHandleCallback = null;
			if (!string.IsNullOrEmpty(response.State))
			{
				if (_memoryCache.TryGetValue(response.State.ToString(), out tikTokHandleCallback))
				{
					_memoryCache.Remove(response.State.ToString());
				}
			}

			if (tikTokHandleCallback == null)
			{
				throw new Exception("Cannot process TikTok login");
			}

			return tikTokHandleCallback.CallbackStrategy switch
			{
				CallbackStrategy.Login => await TikTokLogin(userInfo, tikTokHandleCallback.ReturnUrl),
				CallbackStrategy.AddTikTokUser => await AddTikTokUser(tikTokHandleCallback.ReturnUrl, tokenData, userInfo),
				_ => await TikTokLogin(userInfo, tikTokHandleCallback.ReturnUrl)
			};
		}

		private async Task<IActionResult> TikTokLogin(UserInfo userInfo, string returnUrl)
		{
			var user = await _userService.GetUserByTikTokId(userInfo.UnionId);
			if (user != null)
			{
				await LoginUser(user);
				if (!string.IsNullOrEmpty(returnUrl))
				{
					return Redirect(returnUrl);
				}

				return RedirectToAction("Index", "Posting");
			}

			var model = new LoginModel
			{
				ReturnUrl = returnUrl,
				Error = "You should register and add TikTok user to registered user to be available login via TikTok"
			};
			return View(nameof(Login), model);
		}

		private async Task<IActionResult> AddTikTokUser(string returnUrl, AuthTokenData tokenData, UserInfo userInfo)
		{
			if (!User.Identity.IsAuthenticated)
			{
				var model = new LoginModel
				{
					ReturnUrl = returnUrl
				};
				return View(nameof(Login), model);
			}

			try
			{
				await _userService.AddTikTokUser(UserId, new TikTokUser
				{
					AuthTokenData = tokenData,
					UserInfo = userInfo
				});

				return RedirectToAction("Index", "User");
			}
			catch(ArgumentException ex)
			{
				return RedirectToAction("Index", "User", new { Error = ex.Message });
			}
		}
	}
}