using Microsoft.AspNetCore.Identity;
using Polufabrikkat.Core.Interfaces;
using Polufabrikkat.Core.Models;
using Polufabrikkat.Site.Interfaces;
using Polufabrikkat.Site.Models;

namespace Polufabrikkat.Site.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly PasswordHasher<User> _passwordHasher;

		public UserService(IUserRepository userRepository)
		{
			_userRepository = userRepository;
			_passwordHasher = new PasswordHasher<User>();
		}

		public async Task<User> VerifyUserLogin(LoginModel request)
		{
			var user = await _userRepository.GetUserByUsername(request.Username);
			if (user == null)
			{
				return user;
			}

			var result = _passwordHasher.VerifyHashedPassword(null, user.PasswordHash, request.Password);

			if (result == PasswordVerificationResult.Success)
			{
				return user;
			}

			return null;
		}

		public async Task<User> RegisterUser(LoginModel model)
		{
			var existingUser = await _userRepository.GetUserByUsername(model.Username);
			if (existingUser != null)
			{
				throw new ArgumentException("User with this username is already exists");
			}

			var newUser = new User
			{
				Username = model.Username,
				PasswordHash = _passwordHasher.HashPassword(null, model.Password),
				Id = Guid.NewGuid().ToString(),
			};
			return await _userRepository.CreateUser(newUser);
		}

		public Task<User> GetUserByTikTokId(string unionId)
		{
			return _userRepository.GetUserByTikTokId(unionId);
		}
	}
}
