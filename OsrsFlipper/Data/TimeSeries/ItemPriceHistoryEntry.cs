using System.Text.Json.Serialization;

namespace OsrsFlipper.Data.TimeSeries;

public class ItemPriceHistoryEntry
{
    [JsonPropertyName("timestamp")]
    public string TimestampStr { get; }

    [JsonPropertyName("avgHighPrice")]
    public string AvgHighPriceStr { get; }

    [JsonPropertyName("avgLowPrice")]
    public string AvgLowPriceStr { get; }

    [JsonPropertyName("highPriceVolume")]
    public string HighPriceVolumeStr { get; }

    [JsonPropertyName("lowPriceVolume")]
    public string LowPriceVolumeStr { get; }


    public ItemPriceHistoryEntry(string timestampStr, string avgHighPriceStr, string avgLowPriceStr, string highPriceVolumeStr, string lowPriceVolumeStr)
    {
        TimestampStr = timestampStr;
        AvgHighPriceStr = avgHighPriceStr;
        AvgLowPriceStr = avgLowPriceStr;
        HighPriceVolumeStr = highPriceVolumeStr;
        LowPriceVolumeStr = lowPriceVolumeStr;
    }


    public bool TryGetTimestamp(out DateTime timestamp)
    {
        if (!long.TryParse(TimestampStr, out long timestampUnix))
        {
            timestamp = default;
            return false;
        }
        
        timestamp = Utils.UnixTimeToDateTime(timestampUnix);
        return true;
    }
    
    
    public bool TryGetAvgHighPrice(out int avgHighPrice) => int.TryParse(AvgHighPriceStr, out avgHighPrice);
    public bool TryGetAvgLowPrice(out int avgLowPrice) => int.TryParse(AvgLowPriceStr, out avgLowPrice);
    public bool TryGetHighPriceVolume(out int highPriceVolume) => int.TryParse(HighPriceVolumeStr, out highPriceVolume);
    public bool TryGetLowPriceVolume(out int lowPriceVolume) => int.TryParse(LowPriceVolumeStr, out lowPriceVolume);
}