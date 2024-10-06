using Polufabrikkat.Core.Models.Entities;

namespace Polufabrikkat.Core.Interfaces
{
	public interface IPostService
	{
		Task<Post> AddNewPost(Post post, List<Models.Entities.File> files);
		Task<List<Post>> GetPostsByUserId(string userId);
	}
}
