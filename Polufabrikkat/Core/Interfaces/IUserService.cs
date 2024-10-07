using Polufabrikkat.Core.Models.Entities;
using Polufabrikkat.Core.Models.TikTok;

namespace Polufabrikkat.Core.Interfaces
{
	public interface IUserService
	{
		Task<User> VerifyUserLogin(string username, string password);
		Task<User> RegisterUser(string username, string password);
		Task<User> GetUserByTikTokId(string unionId);
		Task<User> GetUserById(string userId);
		Task UpdateUser(User user);
		Task RemoveTikTokUser(string userId, string tikTokUserUnionId);
		Task AddTikTokUser(string userId, TikTokUser tikTokUser);
		Task<TikTokUser> GetTikTokUserByUnionId(string unionId);
		Task UpdateAuthData(AuthTokenData authData);
	}
}
