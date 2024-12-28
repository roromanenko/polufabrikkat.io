using Polufabrikkat.Core.Models.Entities;

namespace Polufabrikkat.Core.Interfaces
{
	public interface IPostService
	{
		Task<Post> AddNewPost(Post post, List<Models.Entities.File> files);
		Task<Post> GetPostById(string id);
		Task<List<Post>> GetFilteredPosts(
			PostStatus[] statuses = null,
			DateTime? scheduledPublicationTimeFrom = null,
			DateTime? scheduledPublicationTimeTo = null,
			string userId = null);
	}
}
