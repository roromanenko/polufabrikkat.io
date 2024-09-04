using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Polufabrikkat.Site.Options;

namespace Polufabrikkat.Site.Controllers
{
	[Authorize]
	public class PostingController : Controller
	{
		private readonly FileUploadOptions _fileUploadOptions;

		public PostingController(IOptions<FileUploadOptions> fileUploadOptions)
		{
			_fileUploadOptions = fileUploadOptions.Value;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task UploadFile()
		{
			var file = Request.Form.Files.First();
			using var writeStream = System.IO.File.Create(Path.Combine(_fileUploadOptions.FileUploadPath, file.FileName));
			await file.CopyToAsync(writeStream);
		}
	}
}
