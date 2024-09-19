namespace Polufabrikkat.Site.Models
{
	public class TikTokCallbackResponse
	{
		public string Code { get; set; }
		public string Scopes { get; set; }
		public string State { get; set; }
		public string Error { get; set; }
		public string ErrorDescription { get; set; }
		public TikTokCallbackResponse()
		{
		}
		public TikTokCallbackResponse(IQueryCollection query)
		{
			Code = query["code"];
			Scopes = query["scopes"];
			State = query["state"];
			Error = query["error"];
			ErrorDescription = query["error_description"];
		}
	}
}
