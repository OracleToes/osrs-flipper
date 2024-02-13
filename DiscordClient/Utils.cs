using System.Globalization;

namespace DiscordClient;

public static class Utils
{
    public static string SeparateThousands(this int value)
    {
        NumberFormatInfo largeNumberFormat = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        largeNumberFormat.NumberGroupSeparator = " ";
        return value.ToString("#,0", largeNumberFormat);
    }
    
    
    
    public static long DateTimeToUnixTime(DateTime dateTime)
    {
        return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }
}