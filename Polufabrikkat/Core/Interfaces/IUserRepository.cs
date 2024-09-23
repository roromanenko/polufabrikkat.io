using Polufabrikkat.Core.Models.Entities;
using Polufabrikkat.Core.Models.TikTok;

namespace Polufabrikkat.Core.Interfaces
{
    public interface IUserRepository
	{
		Task<User> CreateUser(User newUser);
		Task<User> GetUserById(string userId);
		Task<User> GetUserByTikTokId(string unionId);
		Task<User> GetUserByUsername(string username);
		Task UpdateUser(User user);
		Task RemoveTikTokUser(string userId, string tikTokUserUnionId);
		Task AddTikTokUser(string userId, TikTokUser tikTokUser);
	}
}
