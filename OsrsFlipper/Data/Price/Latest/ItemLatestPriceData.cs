using System.Text.Json.Serialization;

namespace OsrsFlipper.Data.Price.Latest;

public class ItemLatestPriceData
{
    [JsonPropertyName("high")]
    public int? High { get; set; }

    [JsonPropertyName("highTime")]
    public long? HighTimeUnix { get; set; }

    [JsonPropertyName("low")]
    public int? Low { get; set; }

    [JsonPropertyName("lowTime")]
    public long? LowTimeUnix { get; set; }
    
    [JsonIgnore]
    public bool IsValid => High.HasValue && Low.HasValue && HighTimeUnix.HasValue && LowTimeUnix.HasValue;
    
    [JsonIgnore]
    public DateTime HighTime => Utils.UnixTimeToDateTime(HighTimeUnix!.Value);
    
    [JsonIgnore]
    public DateTime LowTime => Utils.UnixTimeToDateTime(LowTimeUnix!.Value);
}