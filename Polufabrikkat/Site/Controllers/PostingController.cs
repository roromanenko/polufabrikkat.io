using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Options;
using Polufabrikkat.Site.Interfaces;
using Polufabrikkat.Site.Models.Posting;
using Polufabrikkat.Site.Models.User;
using Polufabrikkat.Site.Options;

namespace Polufabrikkat.Site.Controllers
{
	[Authorize]
	public class PostingController : BaseController
	{
		private readonly FileUploadOptions _fileUploadOptions;
		private readonly IUserService _userService;
		private readonly ITikTokApiClient _tikTokApiClient;
		private readonly IMapper _mapper;
		private readonly TikTokOptions _tikTokOptions;

		public PostingController(IOptions<FileUploadOptions> fileUploadOptions, IUserService userService,
			ITikTokApiClient tikTokApiClient, IMapper mapper, IOptions<TikTokOptions> tikTokOptions)
		{
			_fileUploadOptions = fileUploadOptions.Value;
			_userService = userService;
			_tikTokApiClient = tikTokApiClient;
			_mapper = mapper;
			_tikTokOptions = tikTokOptions.Value;
		}

		public async Task<IActionResult> Index(PostingViewModel model)
		{
			var user = await _userService.GetUserById(UserId);
			model.TikTokUsers = _mapper.Map<List<TikTokUserModel>>(user.TikTokUsers);
			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> SelectTikTokUser(string unionId)
		{
			var user = await _userService.GetUserByTikTokId(unionId);
			if (user == null)
			{
				return BadRequest(new { error = "TikTok user not found" });
			}

			var tiktokUser = user.TikTokUsers.First(x => x.UserInfo.UnionId == unionId);
			if(tiktokUser.QueryCreatorInfo != null
				&& ((DateTime.UtcNow - tiktokUser.QueryCreatorInfo.RefreshedDateTime) < _tikTokOptions.RefreshQueryCreatorInfoInterval))
			{
				return Ok();
			}

			var queryCreatorInfo = await _tikTokApiClient.GetQueryCreatorInfo(tiktokUser.AuthTokenData);
			queryCreatorInfo.RefreshedDateTime = DateTime.UtcNow;
			tiktokUser.QueryCreatorInfo = queryCreatorInfo;
			await _userService.UpdateUser(user);

			return Ok();
		}
	}
}
