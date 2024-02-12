using OsrsFlipper.Api;
using OsrsFlipper.Caching;
using OsrsFlipper.Data.Mapping;
using OsrsFlipper.Data.Price.Average;
using OsrsFlipper.Data.Price.Latest;
using OsrsFlipper.Filtering;
using OsrsFlipper.Filtering.Filters;

namespace OsrsFlipper;

public sealed class Flipper : IDisposable
{
    /// <summary>
    /// The API controller used to fetch data from the OSRS API.
    /// </summary>
    private readonly OsrsApiController _apiController;
    
    /// <summary>
    /// Contains cached market data for every item in the game.
    /// </summary>
    private readonly ItemCache _cache;
    
    /// <summary>
    /// Manages cooldowns for items (to avoid the same item being detected multiple times in a short period).
    /// </summary>
    private readonly CooldownManager _cooldownManager = new();
    
    /// <summary>
    /// Contains all the filters that an item must pass to be considered for flipping.
    /// </summary>
    private readonly FilterCollection _filterCollection = new();


    private Flipper(OsrsApiController apiController, ItemCache cache)
    {
        _apiController = apiController;
        _cache = cache;
        
        // Add wanted filters to the filter collection.
        _filterCollection
            .AddFilter(new ValidDataFilter())   // Skip items with invalid data.
            .AddFilter(new ItemCooldownFilter(_cooldownManager))    // Skip items that are on a cooldown.
            .AddFilter(new Item24HAveragePriceFilter(50, 50_000_000))   // Skip items with a 24-hour average price outside the range 50 - 50,000,000.
            .AddFilter(new PotentialProfitFilter(200_000, true))    // Skip items with a potential profit less than 200k.
            .AddFilter(new ReturnOfInvestmentFilter(5))     // Skip items with a return of investment less than 5%.
            .AddFilter(new MaxVolatilityFilter(15))     // Skip items with a price fluctuation of more than 15% in the last 5 minutes.
    }
    
    
    /// <summary>
    /// Creates a new Flipper instance.
    /// </summary>
    public static async Task<Flipper> Create()
    {
        OsrsApiController apiController = new();
        
        ItemMapping mapping = await apiController.GetItemMapping();
        
        ItemCache cache = new(mapping);
        
        Flipper flipper = new Flipper(apiController, cache);
        return flipper;
    }
    
    
    /// <summary>
    /// Finds all potential dumps.
    /// </summary>
    /// <returns></returns>
    public async Task<List<ItemFlip>> FindDumps()
    {
        List<ItemFlip> flips = new();
        
        // Loop all available items.
        foreach (CacheEntry entry in _cache.Entries())
        {
            // Skip if the item is on cooldown.
            if (_cooldownManager.IsOnCooldown(entry.Item.Id))
                continue;
            
            // Try to calculate a flip for the item.
            ItemFlip? flip = await TryCalculateFlip(entry);

            // Skip if no flip was found.
            if (flip == null)
                continue;
        
            // Skip if the item is not flippable.
            if (!entry.IsPotentiallyFlippable())
                continue;
            
            // Add the flip to the list and set the item on cooldown.
            flips.Add(flip);
            _cooldownManager.SetCooldown(entry.Item.Id, TimeSpan.FromMinutes(2));
        }
        
        return flips;
    }
    
    
    /// <summary>
    /// Tries to find a flip for the given item.
    /// </summary>
    /// <param name="entry">The item to calculate a flip for.</param>
    /// <returns>A flip if one was found, otherwise null.</returns>
    private static async Task<ItemFlip?> TryCalculateFlip(CacheEntry entry)
    {
        const double minPriceDropPercentage = 12 / 100.0;
        
        // The price the item should be bought at to make a profit.
        int priceToBuyAt = entry.PriceLatest.LowestPrice;
        // The price the item should be sold at to make a profit.
        int priceToSellAt = entry.Price1HourAverage.HighestPrice; // NOTE: Possibly use Price1HourAverage.AveragePrice instead of Price1HourAverage.HighestPrice?
        
        // Calculate the potential profit.
        int margin = priceToSellAt - priceToBuyAt;
        int? potentialProfit = entry.Item.HasBuyLimit ? margin * entry.Item.GeBuyLimit : null;
        
        // Skip if potential profit is less than 200k
        if (potentialProfit is < 200_000)
            return null;
        
        // Check that the lowest component of the latest price has dropped minPriceDropPercentage or more, compared to the last 5 minute average price.
        if (priceToBuyAt > entry.Price5MinAverageOffset.AveragePrice * (1.0 - minPriceDropPercentage))
            return null;
        
        // The current lowest price should also be less than the 1 hour average price.
        if (priceToBuyAt > entry.Price1HourAverage.AveragePrice * (1.0 - minPriceDropPercentage))
            return null;
        
        // Calculate the ROI percentage
        double roiPercentage = margin / (double)priceToBuyAt * 100;
        
        return new ItemFlip(entry.Item, potentialProfit, entry.Price24HourAverage.TotalVolume, roiPercentage, entry.PriceLatest.BuyPrice, entry.PriceLatest.SellPrice, entry.PriceLatest.LastBuyTime, entry.PriceLatest.LastSellTime, entry.Price6HourAverage.AveragePrice);
    }
    
    
    /// <summary>
    /// Refreshes all the latest market data in the cache.
    /// </summary>
    public async Task RefreshCache()
    {
        ItemLatestPriceDataCollection? latestPrices = await _apiController.GetLatestPrices();
        if (latestPrices != null)
            _cache.UpdateLatestPrices(latestPrices);
        else
            Logger.Warn("Failed to load latest prices");
        
        ItemAveragePriceDataCollection? average5MinOffsetPrices = await _apiController.Get5MinAveragePrices(Get5MinOffset());
        if (average5MinOffsetPrices != null)
            _cache.Update5MinAverageOffsetPrices(average5MinOffsetPrices);
        else
            Logger.Warn("Failed to load 5 minute average (offset) prices");
        
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


    private static DateTime Get5MinOffset()
    {
        // Get the 5-minute period we are currently in, and subtract 5 minutes to get the previous period.
        long currentUnixTime = Utils.DateTimeToUnixTime(DateTime.UtcNow);
        long startOfCurrentPeriod = currentUnixTime / 300;
        long startOfLastPeriod = startOfCurrentPeriod - 1;
        return Utils.UnixTimeToDateTime(startOfLastPeriod * 300);
    }


    public void Dispose()
    {
        _apiController.Dispose();
    }
}