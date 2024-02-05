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
    public DateTime Timestamp => Utils.UnixTimeToDateTime(TimestampUnix!.Value);
}