using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Polufabrikkat.Core.Models.Entities;
using System.Security.Claims;

namespace Polufabrikkat.Site.Controllers
{
    public abstract class BaseController : Controller
	{
		protected Task LoginUser(User user)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Username),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			};
			if (user.Roles != null && user.Roles.Any())
			{
				claims.AddRange(user.Roles.Select(x => new Claim(ClaimTypes.Role, x)));
			}

			var claimsIdentity = new ClaimsIdentity(claims, "Login");
			return HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
		}

		protected Task LogoutUser()
		{
			return HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		}

		protected string Username => User.Identity.Name;
		protected string UserId => User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
	}
}
