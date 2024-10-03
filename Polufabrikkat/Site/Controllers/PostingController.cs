using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Polufabrikkat.Core.ApiClients;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models.TikTok;
using Polufabrikkat.Core.Options;
using Polufabrikkat.Site.Helpers;
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
		private readonly IFileRepository _fileRepository;
		private readonly TikTokOptions _tikTokOptions;

		public PostingController(IOptions<FileUploadOptions> fileUploadOptions, IUserService userService,
			ITikTokApiClient tikTokApiClient, IMapper mapper, IOptions<TikTokOptions> tikTokOptions, IFileRepository fileRepository)
		{
			_fileUploadOptions = fileUploadOptions.Value;
			_userService = userService;
			_tikTokApiClient = tikTokApiClient;
			_mapper = mapper;
			_fileRepository = fileRepository;
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
			if (tiktokUser.QueryCreatorInfo != null
				&& ((DateTime.UtcNow - tiktokUser.QueryCreatorInfo.RefreshedDateTime) < _tikTokOptions.RefreshQueryCreatorInfoInterval))
			{
				return Json(_mapper.Map<QueryCreatorInfoModel>(tiktokUser.QueryCreatorInfo));
			}

			QueryCreatorInfo queryCreatorInfo;
			try
			{
				queryCreatorInfo = await _tikTokApiClient.GetQueryCreatorInfo(tiktokUser.AuthTokenData);
			}
			catch (TikTokInvalidTokenException)
			{
				tiktokUser.AuthTokenData = await _tikTokApiClient.RefreshTokenData(tiktokUser.AuthTokenData);
				await _userService.UpdateUser(user);
				queryCreatorInfo = await _tikTokApiClient.GetQueryCreatorInfo(tiktokUser.AuthTokenData);
			}

			queryCreatorInfo.RefreshedDateTime = DateTime.UtcNow;
			tiktokUser.QueryCreatorInfo = queryCreatorInfo;
			await _userService.UpdateUser(user);

			return Json(_mapper.Map<QueryCreatorInfoModel>(tiktokUser.QueryCreatorInfo));
		}

		[HttpPost]
		public async Task<IActionResult> CreateNewPhotoPost([FromForm] NewPhotoPostRequest request)
		{
			// 10 MB size limit in bytes
			const long MaxFileSize = 10 * 1024 * 1024;
			Request.IsHttps = true;

			var fileUrls = new List<string>();
			if (request.Files.Any(x => x.Length > MaxFileSize))
			{
				return BadRequest("File size exceeds 10 MB limit.");
			}
			if (request.Files.Any(x => !PhotoHelper.IsMimeTypeAllowed(x.ContentType)))
			{
				return BadRequest("Allowed only WebP and JPEG formats.");
			}

			foreach (var file in request.Files)
			{
				using var memoryStream = new MemoryStream();
				await file.CopyToAsync(memoryStream);

				var newFile = new Core.Models.Entities.File
				{
					Added = DateTime.UtcNow,
					ContentType = file.ContentType,
					FileName = Guid.NewGuid().ToString() + PhotoHelper.GetExtensionFromMimeType(file.ContentType),
					FileData = memoryStream.ToArray()
				};
				await _fileRepository.SaveFile(newFile);
				var absoluteUrl = Url.Action("Get", "File", new { fileName = newFile.FileName }, Request.Scheme, Request.Host.Value);
				fileUrls.Add(absoluteUrl);
			}

			var apiRequest = new PostPhotoRequest
			{
				MediaType = "PHOTO",
				PostMode = "DIRECT_POST",
				PostInfo = new PostInfo
				{
					Title = request.Title,
					Description = request.Description,
					PrivacyLevel = request.PrivacyLevel,
					DisableComment = request.DisableComment,
					AutoAddMusic = request.AutoAddMusic,
				},
				SourceInfo = new SourceInfo
				{
					Source = "PULL_FROM_URL",
					PhotoCoverIndex = request.PhotoCoverIndex,
					PhotoImages = fileUrls
				}
			};

			var user = await _userService.GetUserByTikTokId(request.TikTokUserUnionId);
			var tiktokUser = user.TikTokUsers.First(x => x.UserInfo.UnionId == request.TikTokUserUnionId);
			var res = await _tikTokApiClient.PostPhoto(tiktokUser.AuthTokenData, apiRequest);

			return Ok(res);
		}
	}
}
