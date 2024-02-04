using System.Text.Json.Serialization;

namespace OsrsFlipper.Data.Price;

public class ItemPriceData
{
    [JsonPropertyName("high")]
    private string HighStr { get; }

    [JsonPropertyName("highTime")]
    private string HighTimeStr { get; }

    [JsonPropertyName("low")]
    private string LowStr { get; }

    [JsonPropertyName("lowTime")]
    private string LowTimeStr { get; }


    public ItemPriceData(string highStr, string highTimeStr, string lowStr, string lowTimeStr)
    {
        HighStr = highStr;
        HighTimeStr = highTimeStr;
        LowStr = lowStr;
        LowTimeStr = lowTimeStr;
    }
    
    
    public bool TryGetHighPrice(out int highPrice) => int.TryParse(HighStr, out highPrice);
    public bool TryGetHighTime(out DateTime highTime)
    {
        if (!long.TryParse(HighTimeStr, out long highTimeUnix))
        {
            highTime = default;
            return false;
        }
        
        highTime = Utils.UnixTimeToDateTime(highTimeUnix);
        return true;
    }


    public bool TryGetLowPrice(out int lowPrice) => int.TryParse(LowStr, out lowPrice);
    public bool TryGetLowTime(out DateTime lowTime)
    {
        if (!long.TryParse(HighTimeStr, out long lowTimeUnix))
        {
            lowTime = default;
            return false;
        }
        
        lowTime = Utils.UnixTimeToDateTime(lowTimeUnix);
        return true;
    }
}