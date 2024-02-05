using System.Text.Json.Serialization;

namespace OsrsFlipper.Data.Price.Average;

public class ItemAveragePriceData
{
    [JsonPropertyName("avgHighPrice")]
    private int? AverageHigh { get; set; }

    [JsonPropertyName("highPriceVolume")]
    private int? HighVolume { get; set; }

    [JsonPropertyName("avgLowPrice")]
    private int? AverageLow { get; set; }

    [JsonPropertyName("lowPriceVolume")]
    private int? LowVolume { get; set; }
    
    [JsonIgnore]
    public bool IsValid => AverageHigh.HasValue && HighVolume.HasValue && AverageLow.HasValue && LowVolume.HasValue;
}