using Polufabrikkat.Core.Models;
using Polufabrikkat.Site.Models;

namespace Polufabrikkat.Site.Interfaces
{
	public interface IUserService
	{
		Task<User> VerifyUserLogin(LoginModel request);
		Task<User> RegisterUser(LoginModel model);
		Task<User> GetUserByTikTokId(string unionId);
	}
}
