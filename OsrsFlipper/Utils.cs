using System.Globalization;
using OsrsFlipper.Api;

namespace OsrsFlipper;

internal static class Utils
{
    public static DateTime UnixTimeToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch.
        DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }


    public static string AsString(this TimeSeriesApi.TimeSeriesTimeStep timeStep)
    {
        return timeStep switch
        {
            TimeSeriesApi.TimeSeriesTimeStep.FiveMinutes => "5m",
            TimeSeriesApi.TimeSeriesTimeStep.Hour => "1h",
            TimeSeriesApi.TimeSeriesTimeStep.SixHours => "6h",
            TimeSeriesApi.TimeSeriesTimeStep.Day => "24h",
            _ => throw new ArgumentOutOfRangeException(nameof(timeStep), timeStep, null)
        };
    }


    public static string SeparateThousands(this int value)
    {
        NumberFormatInfo largeNumberFormat = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        largeNumberFormat.NumberGroupSeparator = " ";
        return value.ToString("#,0", largeNumberFormat);
    }
}