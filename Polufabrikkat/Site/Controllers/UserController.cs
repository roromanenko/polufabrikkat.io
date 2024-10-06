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
		private readonly IPostService _postService;

		public UserController(IMapper mapper, IUserService userService, IPostService postService)
		{
			_mapper = mapper;
			_userService = userService;
			_postService = postService;
		}

		public async Task<IActionResult> Index(UserViewModel model)
		{
			var user = await _userService.GetUserById(UserId);
			model.User = _mapper.Map<UserModel>(user);
			var posts = await _postService.GetPostsByUserId(UserId);
			model.Posts = _mapper.Map<List<PostShortInto>>(posts);

			return View(model);
		}

		[HttpDelete]
		public async Task RemoveTikTokUser(string unionId)
		{
			await _userService.RemoveTikTokUser(UserId, unionId);
		}
	}
}
