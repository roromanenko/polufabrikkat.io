namespace Polufabrikkat.Site.Interfaces
{
	public interface IDateTimeProvider
	{
		public DateTime? ConvertToClienTimezoneFromUtc(DateTime? date);
		public DateTime ConvertToClienTimezoneFromUtc(DateTime date);

		public DateTime? ConvertToUtcFromClientTimezone(DateTime? date);
		public DateTime ConvertToUtcFromClientTimezone(DateTime date);

		public DateTime ClientNow { get; }
		TimeZoneInfo ClientTimeZone { get; }
	}
}
