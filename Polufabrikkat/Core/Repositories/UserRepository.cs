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
    public class UserRepository : IUserRepository
	{
		private readonly MongoClient _mongoClient;
		private readonly MongoDbOptions _mongoDbOptions;
		private readonly IMongoDatabase _database;

		public UserRepository(MongoClient mongoClient, IOptions<MongoDbOptions> mongoDbOptions)
		{
			_mongoClient = mongoClient;
			_mongoDbOptions = mongoDbOptions.Value;
			_database = _mongoClient.GetDatabase(_mongoDbOptions.DatabaseName);
		}

		public async Task<User> CreateUser(User newUser)
		{
			var userCollection = _database.GetCollection<User>();
			await userCollection.InsertOneAsync(newUser);
			return newUser;
		}

		public Task<User> GetUserByTikTokId(string unionId)
		{
			var filter = Builders<User>.Filter.ElemMatch(u => u.TikTokUsers, t => t.UserInfo.UnionId == unionId);
			var userCollection = _database.GetCollection<User>();
			var user = userCollection.Find(filter).FirstOrDefaultAsync();
			return user;
		}

		public Task<User> GetUserByUsername(string username)
		{
			var filter = Builders<User>.Filter.Eq(u => u.Username, username);
			var userCollection = _database.GetCollection<User>();
			var user = userCollection.Find(filter).FirstOrDefaultAsync();
			return user;
		}

		public Task<User> GetUserById(string userId)
		{
			var filter = Builders<User>.Filter.Eq(u => u.Id, ObjectId.Parse(userId));
			var userCollection = _database.GetCollection<User>();
			var user = userCollection.Find(filter).FirstOrDefaultAsync();
			return user;
		}

		public Task UpdateUser(User user)
		{
			var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
			var userCollection = _database.GetCollection<User>();

			return userCollection.ReplaceOneAsync(filter, user);
		}

		public Task RemoveTikTokUser(string userId, string tikTokUserUnionId)
		{
			var userCollection = _database.GetCollection<User>();
			var filter = Builders<User>.Filter.Eq(u => u.Id, ObjectId.Parse(userId));
			var update = Builders<User>.Update.PullFilter(u => u.TikTokUsers, t => t.UserInfo.UnionId == tikTokUserUnionId);
			return userCollection.UpdateOneAsync(filter, update);

		}

		public Task AddTikTokUser(string userId, TikTokUser tikTokUser)
		{
			var userCollection = _database.GetCollection<User>();
			var filter = Builders<User>.Filter.Eq(u => u.Id, ObjectId.Parse(userId));
			var update = Builders<User>.Update.Push(u => u.TikTokUsers, tikTokUser);
			return userCollection.UpdateOneAsync(filter, update);
		}
	}
}
