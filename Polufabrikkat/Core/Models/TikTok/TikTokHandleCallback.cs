namespace Polufabrikkat.Core.Models.TikTok
{
	public class TikTokHandleCallback
	{
		public string ReturnUrl { get; set; }
		public CallbackStrategy CallbackStrategy { get; set; }
	}

	public enum CallbackStrategy
	{
		Login,
		AddTikTokUser
	}
}
