using MongoDB.Bson;
using Polufabrikkat.Core.Models.Entities;

namespace Polufabrikkat.Core.Interfaces
{
	public interface IPostRepository
	{
		Task<Post> AddPost(Post post);
		Task MarkAsSentToTikTok(ObjectId id, string publishId);
	}
}
