using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models.Entities;
using Polufabrikkat.Core.Options;

namespace Polufabrikkat.Core.Services
{
	public class PostService : IPostService
	{
		private readonly IFileRepository _fileRepository;
		private readonly IPostRepository _postRepository;
		private readonly TikTokOptions _tikTokOptions;

		public PostService(IFileRepository fileRepository, IPostRepository postRepository, IOptions<TikTokOptions> tikTokOptions)
		{
			_fileRepository = fileRepository;
			_postRepository = postRepository;
			_tikTokOptions = tikTokOptions.Value;
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
			post.FileUrls = files.Select(x => Path.Combine(_tikTokOptions.PullFileFromBaseUrl, x.FileName)).ToList();
			post = await _postRepository.AddPost(post);

			return post;
		}

		public Task<List<Post>> GetPostsByUserId(string userId)
		{
			return _postRepository.GetPostsByUserId(ObjectId.Parse(userId));
		}
	}
}
