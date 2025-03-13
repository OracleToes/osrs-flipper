using System.Text.Json.Serialization;

namespace OsrsFlipper.Data.Mapping;

/// <summary>
/// Data of an osrs item.
/// </summary>
public class ItemData
{
    [JsonPropertyName("examine")]
    public string ExamineText { get; init; } = null!;

    [JsonPropertyName("id")]
    public int Id { get; init; }
    
    [JsonPropertyName("members")]
    public bool IsMembersOnly { get; init; }
    
    [JsonPropertyName("lowalch")]
    public int LowAlchemyValue { get; init; }
    
    /// <summary>
    /// How many of this item can be bought every 4 hours.
    /// Check <see cref="HasBuyLimit"/> before using this value.
    /// </summary>
    [JsonPropertyName("limit")]
    public int GeBuyLimit { get; init; }
    
    [JsonPropertyName("value")]
    public int Value { get; init; }
    
    [JsonPropertyName("highalch")]
    public int HighAlchemyValue { get; init; }
    
    [JsonPropertyName("icon")]
    public string IconFilename { get; init; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;
    
    public bool HasBuyLimit => GeBuyLimit != 0;
    
    public string OsrsWikiLink => $"https://oldschool.runescape.wiki/w/Special:Lookup?type=item&id={Id}";
    public string OsrsWikiPricesLink => $"https://prices.runescape.wiki/osrs/item/{Id}";
    public string OsrsCloudPricesLink => $"https://prices.osrs.cloud/item/{Id}";
    public string OsrsGeDbLink => $"https://secure.runescape.com/m=itemdb_oldschool/viewitem?obj={Id}";
    public string GeTrackerLink => $"https://www.ge-tracker.com/item/{Id}";
    public string OsrsExchangeLink => $"https://www.osrs.exchange/item/{Name.Replace(" ", "-")}";
    public string RuneCapitalLink => $"https://dev.rune.capital/item/{Id}";

    
    public string GetIconUrl() => $"{Configuration.OSRS_ITEM_IMAGE_URL}/{IconFilename.Replace(" ", "_")}";


    public override string ToString()
    {
        return $"{Name} - {Id}";
    }
}
