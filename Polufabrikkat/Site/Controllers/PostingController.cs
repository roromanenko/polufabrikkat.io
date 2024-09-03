using Microsoft.AspNetCore.Mvc;

namespace Polufabrikkat.Site.Controllers
{
	public partial class HomeController
	{
		public IActionResult Posting()
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
