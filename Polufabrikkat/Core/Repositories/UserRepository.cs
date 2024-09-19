using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Polufabrikkat.Core.Extentions;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models;
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
			var filter = Builders<User>.Filter.Eq(u => u.TikTokUser.UserInfo.UnionId, unionId);
			var userCollection = _database.GetCollection<User>();
			var user =  userCollection.Find(filter).FirstOrDefaultAsync();
			return user;
		}

		public async Task<User> GetUserByUsername(string username)
		{
			var filter = Builders<User>.Filter.Eq(u => u.Username, username);
			var userCollection = _database.GetCollection<User>();
			var user = await userCollection.Find(filter).FirstOrDefaultAsync();
			return user;
		}
	}
}
