using MongoDB.Driver;

namespace Polufabrikkat.Core.Repositories
{
	public class UserRepository
	{
		private readonly MongoClient _mongoClient;

		public UserRepository(MongoClient mongoClient)
		{
			_mongoClient = mongoClient;
		}
	}
}
