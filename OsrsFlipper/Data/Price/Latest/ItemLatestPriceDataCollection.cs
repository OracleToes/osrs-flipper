using System.Text.Json.Serialization;

namespace OsrsFlipper.Data.Price.Latest;

/// <summary>
/// Contains the price data of all osrs items.
/// </summary>
public class ItemLatestPriceDataCollection
{
    /// <summary>
    /// Osrs item price data by item id.
    /// </summary>
    [JsonPropertyName("data")]
    public Dictionary<int, JsonItemLatestPriceData> Data { get; set; } = null!;
}