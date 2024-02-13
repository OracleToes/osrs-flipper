using OsrsFlipper.Data.Mapping;
using OsrsFlipper.Data.Price.Average;
using OsrsFlipper.Data.Price.Latest;

namespace OsrsFlipper.Caching;

/// <summary>
/// Contains the cached real-time market data for every item in the game.
/// </summary>
public class ItemCache
{
    private readonly Dictionary<int, CacheEntry> _cache = new();


    /// <summary>
    /// Enumerates through cached items.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<CacheEntry> Entries() => _cache.Values;


    public ItemCache(ItemMapping mapping)
    {
        foreach (ItemData item in mapping.ItemData())
            _cache.Add(item.Id, new CacheEntry(item));
    }


    public void UpdateLatestPrices(ItemLatestPriceDataCollection latestPrices)
    {
        foreach (KeyValuePair<int, JsonItemLatestPriceData> pair in latestPrices.Data)
        {
            if (_cache.TryGetValue(pair.Key, out CacheEntry? entry))
                entry.UpdateLatestPrices(pair.Value);
        }
    }


    public void Update5MinAverageOffsetPrices(ItemAveragePriceDataCollection prices)
    {
        foreach (KeyValuePair<int, JsonItemAveragePriceData> pair in prices.Data)
        {
            if (_cache.TryGetValue(pair.Key, out CacheEntry? entry))
                entry.Update5MinAverageOffsetPrices(pair.Value);
        }
    }


    public void Update5MinAveragePrices(ItemAveragePriceDataCollection prices)
    {
        foreach (KeyValuePair<int, JsonItemAveragePriceData> pair in prices.Data)
        {
            if (_cache.TryGetValue(pair.Key, out CacheEntry? entry))
                entry.Update5MinAveragePrices(pair.Value);
        }
    }


    public void Update10MinAveragePrices(ItemAveragePriceDataCollection prices)
    {
        foreach (KeyValuePair<int, JsonItemAveragePriceData> pair in prices.Data)
        {
            if (_cache.TryGetValue(pair.Key, out CacheEntry? entry))
                entry.Update10MinAveragePrices(pair.Value);
        }
    }


    public void Update30MinAveragePrices(ItemAveragePriceDataCollection prices)
    {
        foreach (KeyValuePair<int, JsonItemAveragePriceData> pair in prices.Data)
        {
            if (_cache.TryGetValue(pair.Key, out CacheEntry? entry))
                entry.Update30MinAveragePrices(pair.Value);
        }
    }


    public void Update30MinAverageOffsetPrices(ItemAveragePriceDataCollection prices)
    {
        foreach (KeyValuePair<int, JsonItemAveragePriceData> pair in prices.Data)
        {
            if (_cache.TryGetValue(pair.Key, out CacheEntry? entry))
                entry.Update30MinAverageOffsetPrices(pair.Value);
        }
    }


    public void Update1HourAveragePrices(ItemAveragePriceDataCollection prices)
    {
        foreach (KeyValuePair<int, JsonItemAveragePriceData> pair in prices.Data)
        {
            if (_cache.TryGetValue(pair.Key, out CacheEntry? entry))
                entry.Update1HourAveragePrices(pair.Value);
        }
    }


    public void Update6HourAveragePrices(ItemAveragePriceDataCollection prices)
    {
        foreach (KeyValuePair<int, JsonItemAveragePriceData> pair in prices.Data)
        {
            if (_cache.TryGetValue(pair.Key, out CacheEntry? entry))
                entry.Update6HourAveragePrices(pair.Value);
        }
    }


    public void Update24HourAveragePrices(ItemAveragePriceDataCollection prices)
    {
        foreach (KeyValuePair<int, JsonItemAveragePriceData> pair in prices.Data)
        {
            if (_cache.TryGetValue(pair.Key, out CacheEntry? entry))
                entry.Update24HourAveragePrices(pair.Value);
        }
    }
}