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
			return View();
        }

		public IActionResult Privacy()
        {
            return View();
        }

		public IActionResult TermsOfService()
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