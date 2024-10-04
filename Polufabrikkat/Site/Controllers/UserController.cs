using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Site.Models.User;

namespace Polufabrikkat.Site.Controllers
{
	[Authorize]
	public class UserController : BaseController
	{
		private readonly IMapper _mapper;
		private readonly IUserService _userService;

		public UserController(IMapper mapper, IUserService userService)
		{
			_mapper = mapper;
			_userService = userService;
		}

		public async Task<IActionResult> Index(UserViewModel model)
		{
			var user = await _userService.GetUserById(UserId);
			model.User = _mapper.Map<UserModel>(user);

			return View(model);
		}

		[HttpDelete]
		public async Task RemoveTikTokUser(string unionId)
		{
			await _userService.RemoveTikTokUser(UserId, unionId);
		}
	}
}
