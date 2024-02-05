using OsrsFlipper.Api;
using OsrsFlipper.Caching;
using OsrsFlipper.Data.Mapping;
using OsrsFlipper.Data.Price.Average;
using OsrsFlipper.Data.Price.Latest;

namespace OsrsFlipper;

public sealed class Flipper : IDisposable
{
    private readonly OsrsApiController _apiController;
    private readonly ItemCache _cache;
    private readonly CooldownManager _cooldownManager = new();


    private Flipper(OsrsApiController apiController, ItemCache cache)
    {
        _apiController = apiController;
        _cache = cache;
    }
    
    
    public static async Task<Flipper> Create()
    {
        OsrsApiController apiController = new();
        
        ItemMapping mapping = await apiController.GetItemMapping();
        
        ItemCache cache = new(mapping);
        
        Flipper flipper = new Flipper(apiController, cache);
        return flipper;
    }
    
    
    public async Task<List<ItemFlip>> FindDumps()
    {
        List<ItemFlip> flips = new();
        
        foreach (CacheEntry entry in _cache.Entries())
        {
            if (_cooldownManager.IsOnCooldown(entry.Item.Id))
                continue;
            ItemFlip? flip = await TryGetFlip(entry);
            if (flip != null)
            {
                flips.Add(flip);
                _cooldownManager.SetCooldown(entry.Item.Id, TimeSpan.FromMinutes(2));
            }
        }
        
        return flips;
    }
    
    
    private static async Task<ItemFlip?> TryGetFlip(CacheEntry entry)
    {
        const double minPriceDropPercentage = 15 / 100.0;
        if (!entry.IsFlippable())
            return null;
        
        // Calculate the potential profit
        int? potentialProfit = entry.Item.HasBuyLimit ? entry.PriceLatest.MarginWithTax * entry.Item.GeBuyLimit : null;
        
        // Skip if potential profit is less than 100k
        if (potentialProfit is < 100_000)
            return null;
        
        // Check that the lowest component of the latest price has dropped minPriceDropPercentage or more, compared to the last 5 minute average price.
        if (entry.PriceLatest.LowestPrice > entry.Price5MinAverage.AveragePrice * (1.0 - minPriceDropPercentage))
            return null;
        
        return new ItemFlip(entry.Item, potentialProfit, entry.Price24HourAverage.TotalVolume, entry.PriceLatest.BuyPrice, entry.PriceLatest.SellPrice, entry.PriceLatest.LastBuyTime, entry.PriceLatest.LastSellTime, entry.Price6HourAverage.AveragePrice);
    }
    
    
    public async Task RefreshCache()
    {
        ItemLatestPriceDataCollection? latestPrices = await _apiController.GetLatestPrices();
        if (latestPrices != null)
            _cache.UpdateLatestPrices(latestPrices);
        else
            Logger.Warn("Failed to load latest prices");
        
        ItemAveragePriceDataCollection? average5MinPrices = await _apiController.Get5MinAveragePrices();
        if (average5MinPrices != null)
            _cache.Update5MinAveragePrices(average5MinPrices);
        else
            Logger.Warn("Failed to load 5 minute average prices");
        
        ItemAveragePriceDataCollection? average1HourPrices = await _apiController.Get1HourAveragePrices();
        if (average1HourPrices != null)
            _cache.Update1HourAveragePrices(average1HourPrices);
        else
            Logger.Warn("Failed to load 1 hour average prices");
        
        ItemAveragePriceDataCollection? average6HourPrices = await _apiController.Get6HourAveragePrices();
        if (average6HourPrices != null)
            _cache.Update6HourAveragePrices(average6HourPrices);
        else
            Logger.Warn("Failed to load 6 hour average prices");
        
        ItemAveragePriceDataCollection? average24HourPrices = await _apiController.Get24HourAveragePrices();
        if (average24HourPrices != null)
            _cache.Update24HourAveragePrices(average24HourPrices);
        else
            Logger.Warn("Failed to load 24 hour average prices");
    }


    public void Dispose()
    {
        _apiController.Dispose();
    }
}