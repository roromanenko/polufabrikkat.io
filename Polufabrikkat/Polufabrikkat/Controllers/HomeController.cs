using Microsoft.AspNetCore.Mvc;
using Polufabrikkat.Models;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Polufabrikkat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index(IFormFile loadFile)
        {
			LoadImageModel model = new LoadImageModel();
			string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
			if (loadFile != null)
			{
				string fileName = Path.GetFileName(loadFile.FileName);
				string filePath = Path.Combine(dirPath, fileName);
				using (FileStream outputFileStream = new FileStream(filePath, FileMode.Create))
				{
					await loadFile.CopyToAsync(outputFileStream);

				}
			}

			model.FileNames = Directory.GetFiles(dirPath);

			return View(model);
        }

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}