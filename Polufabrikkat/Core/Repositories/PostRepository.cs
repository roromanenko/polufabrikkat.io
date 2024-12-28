using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Polufabrikkat.Core.Extentions;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models.Entities;
using Polufabrikkat.Core.Models.TikTok;
using Polufabrikkat.Core.Options;

namespace Polufabrikkat.Core.Repositories
{
	public class PostRepository : IPostRepository
	{
		private readonly MongoClient _mongoClient;
		private readonly MongoDbOptions _mongoDbOptions;
		private readonly IMongoDatabase _database;
		private readonly IMongoCollection<Post> _postsCollection;

		public PostRepository(MongoClient mongoClient, IOptions<MongoDbOptions> mongoDbOptions)
		{
			_mongoClient = mongoClient;
			_mongoDbOptions = mongoDbOptions.Value;
			_database = _mongoClient.GetDatabase(_mongoDbOptions.DatabaseName);
			_postsCollection = _database.GetCollection<Post>();
		}

		public async Task<Post> AddPost(Post post)
		{
			await _postsCollection.InsertOneAsync(post);
			return post;
		}

		public Task MarkAsSentToTikTok(ObjectId id, string publishId)
		{
			var filter = Builders<Post>.Filter.Eq(u => u.Id, id);
			var update = Builders<Post>.Update
				.Set(x => x.TikTokPublishId, publishId)
				.Set(x => x.Status, PostStatus.SentToTikTok);
			return _postsCollection.UpdateOneAsync(filter, update);
		}

		public Task<Post> GetPostById(ObjectId postId)
		{
			var filter = Builders<Post>.Filter.Eq(u => u.Id, postId);
			return _postsCollection.Find(filter).FirstOrDefaultAsync();
		}

		public Task SetPostStatusData(ObjectId postId, PostStatusData postStatusData)
		{
			var filter = Builders<Post>.Filter.Eq(u => u.Id, postId);
			var update = Builders<Post>.Update.Set(x => x.TikTokPostStatus, postStatusData);

			return _postsCollection.UpdateOneAsync(filter, update);
		}

		public async Task<List<Post>> GetFilteredPosts(
			PostStatus[] statuses = null,
			DateTime? scheduledPublicationTimeFrom = null,
			DateTime? scheduledPublicationTimeTo = null,
			ObjectId? userId = null)
		{
			var filterBuilder = Builders<Post>.Filter;
			var filters = new List<FilterDefinition<Post>>();

			if (statuses != null && statuses.Any())
			{
				filters.Add(filterBuilder.In(p => p.Status, statuses));
			}

			if (scheduledPublicationTimeFrom.HasValue)
			{
				filters.Add(filterBuilder.Gte(p => p.ScheduledPublicationTime, scheduledPublicationTimeFrom.Value));
			}

			if (scheduledPublicationTimeTo.HasValue)
			{
				filters.Add(filterBuilder.Lt(p => p.ScheduledPublicationTime, scheduledPublicationTimeTo.Value));
			}

			if (userId.HasValue)
			{
				filters.Add(filterBuilder.Eq(p => p.UserId, userId.Value));
			}

			var filter = filters.Any() ? filterBuilder.And(filters) : FilterDefinition<Post>.Empty;

			return await _postsCollection.Find(filter).ToListAsync();
		}
	}
}
