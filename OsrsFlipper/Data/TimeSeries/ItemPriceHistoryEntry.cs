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
    
    /// <summary>
    /// Returns the lowest price of buy and sell.
    /// </summary>
    public int? LowestPrice => AvgHighPrice < AvgLowPrice ? AvgHighPrice : AvgLowPrice;
    
    /// <summary>
    /// Returns the highest price of buy and sell.
    /// </summary>
    public int? HighestPrice => AvgHighPrice > AvgLowPrice ? AvgHighPrice : AvgLowPrice;
}