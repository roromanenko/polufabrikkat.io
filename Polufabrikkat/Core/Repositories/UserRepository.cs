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
		private readonly IMongoCollection<User> _userCollection;

		public UserRepository(MongoClient mongoClient, IOptions<MongoDbOptions> mongoDbOptions)
		{
			_mongoClient = mongoClient;
			_mongoDbOptions = mongoDbOptions.Value;
			_database = _mongoClient.GetDatabase(_mongoDbOptions.DatabaseName);
			_userCollection = _database.GetCollection<User>();
		}

		public async Task<User> CreateUser(User newUser)
		{
			await _userCollection.InsertOneAsync(newUser);
			return newUser;
		}

		public Task<User> GetUserByTikTokId(string unionId)
		{
			var filter = Builders<User>.Filter.ElemMatch(u => u.TikTokUsers, t => t.UserInfo.UnionId == unionId);
			var user = _userCollection.Find(filter).FirstOrDefaultAsync();
			return user;
		}

		public Task<User> GetUserByUsername(string username)
		{
			var filter = Builders<User>.Filter.Eq(u => u.Username, username);
			var user = _userCollection.Find(filter).FirstOrDefaultAsync();
			return user;
		}

		public Task<User> GetUserById(string userId)
		{
			var filter = Builders<User>.Filter.Eq(u => u.Id, ObjectId.Parse(userId));
			var user = _userCollection.Find(filter).FirstOrDefaultAsync();
			return user;
		}

		public Task UpdateUser(User user)
		{
			var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);

			return _userCollection.ReplaceOneAsync(filter, user);
		}

		public Task RemoveTikTokUser(string userId, string tikTokUserUnionId)
		{
			var filter = Builders<User>.Filter.Eq(u => u.Id, ObjectId.Parse(userId));
			var update = Builders<User>.Update.PullFilter(u => u.TikTokUsers, t => t.UserInfo.UnionId == tikTokUserUnionId);
			return _userCollection.UpdateOneAsync(filter, update);

		}

		public async Task AddTikTokUser(string userId, TikTokUser tikTokUser)
		{
			var userCollection = _database.GetCollection<User>();
			var filterTikTokUser = Builders<User>.Filter.ElemMatch(u => u.TikTokUsers, t => t.UserInfo.UnionId == tikTokUser.UserInfo.UnionId);
			var userWithExistingTikTokAccount = await _userCollection.Find(filterTikTokUser).FirstOrDefaultAsync();
			if (userWithExistingTikTokAccount != null)
			{
				throw new ArgumentException("This tiktok user is already added to another account");
			}

			var filter = Builders<User>.Filter.Eq(u => u.Id, ObjectId.Parse(userId));
			var update = Builders<User>.Update.Push(u => u.TikTokUsers, tikTokUser);
			await userCollection.UpdateOneAsync(filter, update);
		}

		public Task UpdateAuthData(AuthTokenData authData)
		{
			var filter = Builders<User>.Filter.ElemMatch(u => u.TikTokUsers, tu => tu.AuthTokenData.OpenId == authData.OpenId);
			// Update the first tiktok user mathed to filter
			var update = Builders<User>.Update.Set("TikTokUsers.$.AuthTokenData", authData);

			return _userCollection.UpdateOneAsync(filter, update);

		}
	}
}
