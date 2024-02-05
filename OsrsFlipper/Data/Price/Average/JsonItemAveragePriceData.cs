using System.Text.Json.Serialization;

namespace OsrsFlipper.Data.Price.Average;

public class JsonItemAveragePriceData
{
    [JsonPropertyName("avgHighPrice")]
    public int? AverageHighPrice { get; set; }

    [JsonPropertyName("highPriceVolume")]
    public int? HighVolume { get; set; }

    [JsonPropertyName("avgLowPrice")]
    public int? AverageLowPrice { get; set; }

    [JsonPropertyName("lowPriceVolume")]
    public int? LowVolume { get; set; }
}