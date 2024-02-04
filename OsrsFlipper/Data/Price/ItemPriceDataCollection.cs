using System.Text.Json.Serialization;

namespace OsrsFlipper.Data.Price;

/// <summary>
/// Contains the price data of all osrs items.
/// </summary>
public class ItemPriceDataCollection
{
    /// <summary>
    /// Osrs item price data by item id.
    /// </summary>
    [JsonPropertyName("data")]
    private Dictionary<int, ItemPriceData> Data { get; }
    
    
    public ItemPriceDataCollection(Dictionary<int, ItemPriceData> data)
    {
        Data = data;
    }
    
    
    public bool TryGetItemPriceData(int itemId, out ItemPriceData? itemPriceData) => Data.TryGetValue(itemId, out itemPriceData);
}