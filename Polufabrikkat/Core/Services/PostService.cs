using MongoDB.Bson;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models.Entities;

namespace Polufabrikkat.Core.Services
{
	public class PostService : IPostService
	{
		private readonly IFileRepository _fileRepository;
		private readonly IPostRepository _postRepository;

		public PostService(IFileRepository fileRepository, IPostRepository postRepository)
		{
			_fileRepository = fileRepository;
			_postRepository = postRepository;
		}

		public async Task<Post> AddNewPost(Post post, List<Models.Entities.File> files)
		{
			var fileIds = new List<ObjectId>();
			foreach (var file in files)
			{
				var addedFile = await _fileRepository.SaveFile(file);
				fileIds.Add(addedFile.Id);
			}
			post.FileIds = fileIds;
			post = await _postRepository.AddPost(post);

			return post;
		}

		public Task<List<Post>> GetPostsByUserId(string userId)
		{
			return _postRepository.GetPostsByUserId(ObjectId.Parse(userId));
		}
	}
}
