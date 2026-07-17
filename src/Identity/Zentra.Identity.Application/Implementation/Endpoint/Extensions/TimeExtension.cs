namespace Zentra.Service.Implementation.Endpoint.Extensions;

public static class TimeExtension
{
    public static long ToUnixTime(this DateTime time)
    {
        var unixEpoch = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
        var totalSeconds = time.Subtract(unixEpoch).TotalSeconds;
        if (totalSeconds < 0) return 0;

        return Convert.ToInt32(totalSeconds);
    }

    public static DateTime ToDateTime(this long unixTime)
    {
        var unixEpoch = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
        return unixEpoch.Add(TimeSpan.FromSeconds(unixTime));
    }
}
