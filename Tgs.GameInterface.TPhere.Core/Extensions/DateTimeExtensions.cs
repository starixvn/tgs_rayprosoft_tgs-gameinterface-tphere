namespace Tgs.GameInterface.TPhere.Core.Extensions;

public static class DateTimeExtensions
{
	public static string DatetimeConvertToString(this DateTimeOffset date)
	{
		var _timeZone = TimeZoneInfo.CreateCustomTimeZone(id: "eastern time", baseUtcOffset: TimeSpan.FromHours(-4), "eastern time", "eastern time");
		return TimeZoneInfo.ConvertTime(date.ToUniversalTime(), _timeZone).ToString("yyyy-MM-dd HH:mm:ss");
	}

	public static DateTimeOffset StringConvertToDatetime(this string str)
	{
		var _timeZone = TimeZoneInfo.CreateCustomTimeZone(id: "eastern time", baseUtcOffset: TimeSpan.FromHours(-4), "eastern time", "eastern time");
		if (string.IsNullOrEmpty(str))
		{
			return DateTime.UnixEpoch;
		}

		var date = DateTime.Parse(str);

		return new DateTimeOffset(date, _timeZone.GetUtcOffset(date));
	}
}
