using OsrsFlipper.Api;
using OsrsFlipper.Caching;
using OsrsFlipper.Data.Mapping;
using OsrsFlipper.Data.Price.Average;
using OsrsFlipper.Data.Price.Latest;
using OsrsFlipper.Data.TimeSeries;

namespace OsrsFlipper;

public sealed class Flipper : IDisposable
{
    private readonly OsrsApiController _apiController;
    private readonly ItemCache _cache;


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
    
    
    public List<ItemFlip> FindFlips()
    {
        List<ItemFlip> flips = new();
        
        foreach (CacheEntry entry in _cache.Entries())
        {
            ItemFlip? flip = IsFlip(entry);
            if (flip != null)
                flips.Add(flip);
        }
        
        return flips;
    }
    
    
    private static ItemFlip? IsFlip(CacheEntry entry)
    {
        /*// Skip if the item has not been traded enough in the last hour
        if (entry.InstaBuyCountLastHour < 10 || entry.InstaSellCountLastHour < 10)
            return null;*/
        
        //TODO: Check if item is on cooldown
        
        // Skip if item price is under 200gp
        if (entry.AverageInstaBuyPriceLast24Hours < 200 || entry.AverageInstaSellPriceLast24Hours < 200)
            return null;
        
        // Skip if the data is too old
        if (entry.LastInstaBuyTime < DateTime.Now.AddMinutes(-5) || entry.LastInstaSellTime < DateTime.Now.AddMinutes(-2))
            return null;
        
        // Calculate current margin
        int margin = entry.InstaBuyPrice - entry.InstaSellPrice;
        
        // Calculate the return of investment percentage
        double returnOfInvestment = margin / (double)entry.InstaSellPrice * 100;

        // Skip if ROI is less than 10%
        if (returnOfInvestment < 10)
            return null;
        
        // Calculate the potential profit
        int? potentialProfit = entry.Item.HasBuyLimit ? margin * entry.Item.GeBuyLimit : null;
        
        // Skip if potential profit is less than 100k
        if (potentialProfit is < 100_000)
            return null;
        
        // Calculate how much the price normally fluctuates
        int standardFluctuation = entry.AverageInstaBuyPriceLastHour - entry.AverageInstaSellPriceLastHour;
        
        // Get the fluctuation as a percentage of the buy price
        double standardFluctuationPercentage = standardFluctuation / (double)entry.AverageInstaBuyPriceLastHour * 100;
        
        // Skip if the fluctuation is more than 5%, to get rid of items that are too volatile
        if (standardFluctuationPercentage > 5)
            return null;
        
        // Detect if the item has just dropped in price (crashed/dumped), to avoid buying items that are on a downtrend.
        bool isSellCrash = entry.InstaSellPrice < entry.AverageInstaSellPriceLastHour * 0.85;
        bool isSellRecent = entry.LastInstaBuyTime >= DateTime.Now.AddMinutes(-2);
        if (isSellCrash && isSellRecent)
            return new ItemFlip(entry.Item, potentialProfit, returnOfInvestment);

        bool isBuyCrash = entry.InstaBuyPrice < entry.AverageInstaBuyPriceLastHour * 0.85;
        bool isBuyRecent = entry.LastInstaSellTime >= DateTime.Now.AddMinutes(-2);
        if (isBuyCrash && isBuyRecent)
            return new ItemFlip(entry.Item, potentialProfit, returnOfInvestment);
        
        return null;
    }
    
    
    public async Task RefreshCache()
    {
        ItemLatestPriceDataCollection? latestPrices = await _apiController.GetLatestPrices();
        if (latestPrices != null)
            _cache.UpdateLatestPrices(latestPrices);
        else
            throw new Exception("Failed to load latest prices");
        
        ItemAveragePriceDataCollection? average5MinPrices = await _apiController.Get5MinAveragePrices();
        if (average5MinPrices != null)
            _cache.Update5MinAveragePrices(average5MinPrices);
        else
            throw new Exception("Failed to load 5 minute average prices");
        
        ItemAveragePriceDataCollection? average1HourPrices = await _apiController.Get1HourAveragePrices();
        if (average1HourPrices != null)
            _cache.Update1HourAveragePrices(average1HourPrices);
        else
            throw new Exception("Failed to load 1 hour average prices");
        
        ItemAveragePriceDataCollection? average24HourPrices = await _apiController.Get24HourAveragePrices();
        if (average24HourPrices != null)
            _cache.Update24HourAveragePrices(average24HourPrices);
        else
            throw new Exception("Failed to load 24 hour average prices");
    }


    public void Dispose()
    {
        _apiController.Dispose();
    }
}