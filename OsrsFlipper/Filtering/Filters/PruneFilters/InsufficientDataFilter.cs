using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering.Filters.PruneFilters;

/// <summary>
/// Checks if the item has valid price data.
/// If any of the price data is insufficient, the item is not considered for flipping.
/// </summary>
internal class InsufficientDataFilter : PruneFilter
{
    protected override bool CanPassFilter(CacheEntry itemData)
    {
        bool latestValid = itemData.PriceLatest.IsValid;
        bool average5MinValid = itemData.Price5MinAverage.IsValid;
        bool average5MinOffsetValid = itemData.Price5MinAverageOffset.IsValid;
        bool average10MinValid = itemData.Price10MinAverage.IsValid;
        bool average30MinValid = itemData.Price30MinAverage.IsValid;
        bool average1HValid = itemData.Price1HourAverage.IsValid;
        bool average6HValid = itemData.Price6HourAverage.IsValid;
        bool average24HValid = itemData.Price24HourAverage.IsValid;
        
        bool allDataValid = latestValid &&
                              average5MinValid &&
                              average5MinOffsetValid &&
                              average10MinValid &&
                              average30MinValid &&
                              average1HValid &&
                              average6HValid &&
                              average24HValid;
        
        /*if (!latestValid)
            Logger.Verbose($"Item {itemData.Item} has invalid latest data.");
        if (!average5MinValid)
            Logger.Verbose($"Item {itemData.Item} has invalid 5min average data.");
        if (!average5MinOffsetValid)
            Logger.Verbose($"Item {itemData.Item} has invalid 5min average offset data.");
        if (!average10MinValid)
            Logger.Verbose($"Item {itemData.Item} has invalid 10min average data.");
        if (!average30MinValid)
            Logger.Verbose($"Item {itemData.Item} has invalid 30min average data.");
        if (!average1HValid)
            Logger.Verbose($"Item {itemData.Item} has invalid 1h average data.");
        if (!average6HValid)
            Logger.Verbose($"Item {itemData.Item} has invalid 6h average data.");
        if (!average24HValid)
            Logger.Verbose($"Item {itemData.Item} has invalid 24h average data.");*/
        
        return allDataValid;
    }
}