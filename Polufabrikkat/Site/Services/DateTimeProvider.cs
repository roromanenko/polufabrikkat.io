using Polufabrikkat.Site.Constants;
using Polufabrikkat.Site.Interfaces;

namespace Polufabrikkat.Site.Services
{
	public class DateTimeProvider : IDateTimeProvider
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public TimeZoneInfo ClientTimeZone => _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(DateTimeProviderConstants.TimeZoneKey, out var timeZone)
			? TimeZoneInfo.TryConvertIanaIdToWindowsId(timeZone, out string windowsTimeZoneId)
				? TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId)
				: TimeZoneInfo.Local
			: TimeZoneInfo.Local;

		public DateTime ClientNow => ConvertToClienTimezoneFromUtc(DateTime.UtcNow);

		public DateTimeProvider(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public DateTime? ConvertToClienTimezoneFromUtc(DateTime? date)
		{
			if (!date.HasValue)
			{
				return default;
			}

			return ConvertToClienTimezoneFromUtc(date.Value);
		}

		public DateTime ConvertToClienTimezoneFromUtc(DateTime date)
		{
			return TimeZoneInfo.ConvertTimeFromUtc(date, ClientTimeZone);
		}

		public DateTime? ConvertToUtcFromClientTimezone(DateTime? date)
		{
			if (!date.HasValue)
			{
				return default;
			}

			return ConvertToUtcFromClientTimezone(date.Value);
		}

		public DateTime ConvertToUtcFromClientTimezone(DateTime date)
		{
			return TimeZoneInfo.ConvertTimeToUtc(date, ClientTimeZone);
		}
	}
}
