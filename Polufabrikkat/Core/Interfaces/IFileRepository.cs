

namespace Polufabrikkat.Core.Interfaces
{
	public interface IFileRepository
	{
		Task<Models.Entities.File> SaveFile(Models.Entities.File file);
		Task<Models.Entities.File> GetFileById(string id);
		Task<Models.Entities.File> GetFileByName(string fileName);
		Task<List<string>> GetAllFileNames();
	}
}
