using System.Text.Json.Serialization;

namespace OsrsFlipper.Data.Price.Latest;

public class JsonItemLatestPriceData
{
    [JsonPropertyName("high")]
    public int? HighPrice { get; set; }

    [JsonPropertyName("highTime")]
    public long? HighTimeUnix { get; set; }

    [JsonPropertyName("low")]
    public int? LowPrice { get; set; }

    [JsonPropertyName("lowTime")]
    public long? LowTimeUnix { get; set; }
    
    [JsonIgnore]
    public DateTime HighTime => Utils.UnixTimeToDateTime(HighTimeUnix!.Value);
    
    [JsonIgnore]
    public DateTime LowTime => Utils.UnixTimeToDateTime(LowTimeUnix!.Value);
}