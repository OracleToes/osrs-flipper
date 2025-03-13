using System.Globalization;
using OsrsFlipper.Api;

namespace OsrsFlipper;

internal static class Utils
{
    public static DateTime UnixTimeToDateTime(long unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch.
        DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime;
    }
    
    
    
    public static long DateTimeToUnixTime(DateTime dateTime)
    {
        return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }


    public static string AsString(this TimeSeriesTimeStep timeStep)
    {
        return timeStep switch
        {
            TimeSeriesTimeStep.FiveMinutes => "5m",
            TimeSeriesTimeStep.Hour => "1h",
            TimeSeriesTimeStep.SixHours => "6h",
            TimeSeriesTimeStep.Day => "24h",
            _ => throw new ArgumentOutOfRangeException(nameof(timeStep), timeStep, null)
        };
    }


    public static string SeparateThousands(this int value)
    {
        NumberFormatInfo largeNumberFormat = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        largeNumberFormat.NumberGroupSeparator = ",";
        return value.ToString("#,0", largeNumberFormat);
    }
}
