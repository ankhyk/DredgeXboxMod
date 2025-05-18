using System;

public static class TimeHelper
{
	public static string FormattedTimeString(long time)
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		dateTime = dateTime.AddSeconds((double)time);
		return dateTime.ToLocalTime().ToString("H:mm:ss");
	}

	public static long GetEpochTimeNow()
	{
		return DateTimeOffset.Now.ToUnixTimeSeconds();
	}
}
