using Polufabrikkat.Core.Models.Entities;
using Polufabrikkat.Core.Models.TikTok;
using Polufabrikkat.Site.Models;

namespace Polufabrikkat.Site.Interfaces
{
    public interface IUserService
	{
		Task<User> VerifyUserLogin(LoginModel request);
		Task<User> RegisterUser(LoginModel model);
		Task<User> GetUserByTikTokId(string unionId);
		Task<User> GetUserById(string userId);
		Task UpdateUser(User user);
		Task RemoveTikTokUser(string userId, string tikTokUserUnionId);
		Task AddTikTokUser(string userId, TikTokUser tikTokUser);
	}
}
