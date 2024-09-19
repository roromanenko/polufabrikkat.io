using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Polufabrikkat.Core.Models;
using System.Security.Claims;

namespace Polufabrikkat.Site.Controllers
{
	public abstract class BaseController : Controller
	{
		public Task LoginUser(User user)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Username)
			};
			var claimsIdentity = new ClaimsIdentity(claims, "Login");
			return HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
		}

		public Task LogoutUser()
		{
			return HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		}
	}
}
