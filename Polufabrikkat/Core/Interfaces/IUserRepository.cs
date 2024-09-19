
using Polufabrikkat.Core.Models;

namespace Polufabrikkat.Core.Interfaces
{
	public interface IUserRepository
	{
		Task<User> CreateUser(User newUser);
		Task<User> GetUserByTikTokId(string unionId);
		Task<User> GetUserByUsername(string username);
	}
}
