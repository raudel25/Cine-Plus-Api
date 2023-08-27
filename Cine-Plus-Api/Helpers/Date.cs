namespace Cine_Plus_Api.Helpers;

public static class Date
{
    public static bool Available(long date)
    {
        var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(date);
        var dateTime = dateTimeOffset.DateTime;

        return DateTime.UtcNow <= dateTime;
    }

    public static bool Available(DateTime date)
    {
        return DateTime.UtcNow <= date;
    }

    public static long NowLong()
    {
        var now = DateTime.UtcNow;
        return ((DateTimeOffset)now).ToUnixTimeSeconds();
    }

    public static DateTime LongToDateTime(long date)
    {
        var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(date);
        return dateTimeOffset.DateTime;
    }
}