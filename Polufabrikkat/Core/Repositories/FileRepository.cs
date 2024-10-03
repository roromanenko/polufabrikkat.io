using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Polufabrikkat.Core.Extentions;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Options;

namespace Polufabrikkat.Core.Repositories
{
	public class FileRepository : IFileRepository
	{
		private readonly MongoClient _mongoClient;
		private readonly MongoDbOptions _mongoDbOptions;
		private readonly IMongoDatabase _database;
		private readonly IMongoCollection<Models.Entities.File> _filesCollection;

		public FileRepository(MongoClient mongoClient, IOptions<MongoDbOptions> mongoDbOptions)
		{
			_mongoClient = mongoClient;
			_mongoDbOptions = mongoDbOptions.Value;
			_database = _mongoClient.GetDatabase(_mongoDbOptions.DatabaseName);
			_filesCollection = _database.GetCollection<Models.Entities.File>();
		}

		public async Task<Models.Entities.File> GetFileById(string id)
		{
			return await _filesCollection.Find(f => f.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();
		}

		public async Task<Models.Entities.File> GetFileByName(string fileName)
		{
			return await _filesCollection.Find(f => f.FileName == fileName).FirstOrDefaultAsync();
		}

		public async Task<Models.Entities.File> SaveFile(Models.Entities.File file)
		{
			await _filesCollection.InsertOneAsync(file);
			return file;
		}

		public async Task<List<string>> GetAllFileNames()
		{
			return await _filesCollection
				.Find(FilterDefinition<Models.Entities.File>.Empty)
				.Project(file => file.FileName)
				.ToListAsync();
		}
	}
}
