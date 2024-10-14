using MongoDB.Bson;
using Polufabrikkat.Core.Models.Entities;
using Polufabrikkat.Core.Models.TikTok;

namespace Polufabrikkat.Core.Interfaces
{
	public interface IPostRepository
	{
		Task<Post> AddPost(Post post);
		Task<Post> GetPostById(ObjectId postId);
		Task<List<Post>> GetPostsByUserId(ObjectId userId);
		Task MarkAsSentToTikTok(ObjectId id, string publishId);
		Task SetPostStatusData(ObjectId postId, PostStatusData postStatusData);
	}
}
