using System.Text.Json.Serialization;

namespace OsrsFlipper.Data.TimeSeries;

/// <summary>
/// A list of the high and low prices and trade volumes of an item, at a given interval, up to 365 entries maximum.
/// </summary>
public class ItemPriceHistory
{
    [JsonPropertyName("data")]
    public List<ItemPriceHistoryEntry>? Data { get; }
    
    
    public ItemPriceHistory(List<ItemPriceHistoryEntry>? data)
    {
        Data = data;
    }
}