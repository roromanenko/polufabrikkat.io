using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Polufabrikkat.Core.Constants;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Site.Options;

namespace Polufabrikkat.Site.Controllers
{
	[Authorize(Roles = $"{AppRoles.Admin}")]
	public class AdminController : BaseController
	{
		private readonly FileUploadOptions _fileUploadOptions;
		private readonly IFileRepository _fileRepository;

		public AdminController(IOptions<FileUploadOptions> fileUploadOptions, IFileRepository fileRepository)
		{
			_fileUploadOptions = fileUploadOptions.Value;
			_fileRepository = fileRepository;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult GetAllFiles()
		{
			string[] files = Directory.GetFiles(_fileUploadOptions.FileUploadPath);
			return Json(files);
		}

		public async Task<IActionResult> GetAllFilesFromDb()
		{
			var files = await _fileRepository.GetAllFileNames();
			return Json(files);
		}

		[HttpPost]
		public async Task<IActionResult> UploadFile(IFormFile file)
		{
			// 10 MB size limit in bytes
			const long MaxFileSize = 10 * 1024 * 1024;

			// Check if a file was provided
			if (file == null || file.Length == 0)
			{
				return BadRequest("No file uploaded.");
			}

			if (file.Length > MaxFileSize)
			{
				return BadRequest("File size exceeds 10 MB limit.");
			}

			using var memoryStream = new MemoryStream();
			await file.CopyToAsync(memoryStream);

			var newFile = await _fileRepository.SaveFile(new Core.Models.Entities.File
			{
				ContentType = file.ContentType,
				FileName = file.FileName,
				FileData = memoryStream.ToArray()
			});

			return Ok(new { fileName = file.FileName, newFile });
		}
	}
}
