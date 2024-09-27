using Polufabrikkat.Core.Interfaces;

namespace Polufabrikkat.Core.Repositories
{
	public class FileRepository : IFileRepository
	{
		public FileRepository()
		{
		}

		public async Task SaveFile(Stream stream, string filePath)
		{
			using var fileStrem = new FileStream(filePath, FileMode.Create);
			await stream.CopyToAsync(fileStrem);
		}
	}
}
