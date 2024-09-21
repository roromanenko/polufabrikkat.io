using System.ComponentModel.DataAnnotations;

namespace Polufabrikkat.Site.Models
{
	public class LoginModel : BaseModel
	{
		[Required(ErrorMessage = "Username is required.")]
		public string Username { get; set; }
		[Required(ErrorMessage = "Password is required.")]
		public string Password { get; set; }
		[Required(ErrorMessage = "Confirm password is required.")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		[Display(Name = "Confirm Password")]
		public string ConfirmPassword { get; set; }
		public string ReturnUrl { get; set; }
    }
}
