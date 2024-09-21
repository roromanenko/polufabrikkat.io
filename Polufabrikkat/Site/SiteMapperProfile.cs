using AutoMapper;
using Polufabrikkat.Core.Models;
using Polufabrikkat.Core.Models.TikTok;
using Polufabrikkat.Site.Models.User;

namespace Polufabrikkat.Site
{
	public class SiteMapperProfile : Profile
	{
		public SiteMapperProfile()
		{
			CreateMap<User, UserModel>();
			CreateMap<TikTokUser, TikTokUserModel>()
				.ForMember(x => x.DisplayName, act => act.MapFrom(scr => scr.UserInfo.DisplayName))
				.ForMember(x => x.UnionId, act => act.MapFrom(scr => scr.UserInfo.UnionId));
		}
	}
}
