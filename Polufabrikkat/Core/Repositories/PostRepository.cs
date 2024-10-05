﻿using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Polufabrikkat.Core.Extentions;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models.Entities;
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
	}
}