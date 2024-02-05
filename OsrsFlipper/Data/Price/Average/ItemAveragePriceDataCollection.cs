using System.Text.Json.Serialization;

namespace OsrsFlipper.Data.Price.Average;

/// <summary>
/// Contains the price data of all osrs items.
/// </summary>
public class ItemAveragePriceDataCollection
{
    /// <summary>
    /// Osrs item price data by item id.
    /// </summary>
    [JsonPropertyName("data")]
    public Dictionary<int, ItemAveragePriceData> Data { get; set; } = null!;
}