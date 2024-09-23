using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Polufabrikkat.Core.Constants;
using Polufabrikkat.Site.Options;

namespace Polufabrikkat.Site.Controllers
{
	[Authorize(Roles = $"{AppRoles.Admin}")]
	public class AdminController : BaseController
	{
		private readonly FileUploadOptions _fileUploadOptions;

		public AdminController(IOptions<FileUploadOptions> fileUploadOptions)
		{
			_fileUploadOptions = fileUploadOptions.Value;
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

		[HttpPost]
		public async Task<IActionResult> UploadFile(IFormFile file)
		{
			// Check if a file was provided
			if (file == null || file.Length == 0)
			{
				return BadRequest("No file uploaded.");
			}

			// Create the full file path
			var filePath = Path.Combine(_fileUploadOptions.FileUploadPath, file.FileName);

			// Save the file to the server
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			return Ok(new { fileName = file.FileName });
		}
	}
}
