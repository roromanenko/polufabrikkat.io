

namespace Polufabrikkat.Core.Interfaces
{
	public interface IFileRepository
	{
		Task SaveFile(Stream stream, string filePath);
	}
}
