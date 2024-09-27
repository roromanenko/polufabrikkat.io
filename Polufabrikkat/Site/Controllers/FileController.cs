using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Polufabrikkat.Core.Interfaces;

namespace Polufabrikkat.Site.Controllers
{
	[Route("File")]
	public class FileController : BaseController
	{
		private readonly IFileRepository _fileRepository;

		public FileController(IFileRepository fileRepository)
		{
			_fileRepository = fileRepository;
		}

		[AllowAnonymous]
		[HttpGet("{fileName}")]
		public async Task<IActionResult> Get(string fileName)
		{
			var file = await _fileRepository.GetFileByName(fileName);

			return File(file.FileData, file.ContentType);
		} 
	}
}
