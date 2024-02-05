using System.Text.Json.Serialization;

namespace OsrsFlipper.Data.TimeSeries;

public class ItemPriceHistoryEntry
{
    [JsonPropertyName("timestamp")]
    public long? TimestampUnix { get; set; }

    [JsonPropertyName("avgHighPrice")]
    public int? AvgHighPrice { get; set; }

    [JsonPropertyName("avgLowPrice")]
    public int? AvgLowPrice { get; set; }

    [JsonPropertyName("highPriceVolume")]
    public int? HighPriceVolume { get; set; }

    [JsonPropertyName("lowPriceVolume")]
    public int? LowPriceVolume { get; set; }
    
    [JsonIgnore]
    public bool IsValid => TimestampUnix.HasValue && AvgHighPrice.HasValue && AvgLowPrice.HasValue && HighPriceVolume.HasValue && LowPriceVolume.HasValue;
    
    [JsonIgnore]
    public DateTime Timestamp => Utils.UnixTimeToDateTime(TimestampUnix!.Value);
}