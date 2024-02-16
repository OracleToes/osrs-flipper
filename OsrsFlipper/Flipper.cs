using OsrsFlipper.Api;
using OsrsFlipper.Caching;
using OsrsFlipper.Data.Mapping;
using OsrsFlipper.Data.Price.Average;
using OsrsFlipper.Data.Price.Latest;
using OsrsFlipper.Data.TimeSeries;
using OsrsFlipper.Filtering;
using OsrsFlipper.Filtering.Filters.PruneFilters;

namespace OsrsFlipper;

public sealed class Flipper : IDisposable
{
    private const bool DEBUG_FILTERS = false;
    
    /// <summary>
    /// The API controller used to fetch data from the OSRS API.
    /// </summary>
    private readonly OsrsApiController _apiController;
    
    /// <summary>
    /// Contains cached market data for every item in the game.
    /// </summary>
    private readonly ItemCache _cache;

    /// <summary>
    /// For how long an item should be on cooldown after being detected as a potential flip.
    /// </summary>
    private readonly int _cooldownMinutes;

    /// <summary>
    /// Manages cooldowns for items (to avoid the same item being detected multiple times in a short period).
    /// </summary>
    private readonly CooldownManager _cooldownManager = new();
    
    /// <summary>
    /// Contains all the filters that an item must pass to be considered for flipping.
    /// </summary>
    private readonly FilterCollection _filterCollection = new();


    private Flipper(OsrsApiController apiController, ItemCache cache, int cooldownMinutes)
    {
        _apiController = apiController;
        _cache = cache;
        _cooldownMinutes = cooldownMinutes;

        // Add wanted filters to the filter collection.
        // Prune filters are used to quickly discard items that are not worth considering, and to avoid fetching additional API data for them.
        _filterCollection
            .AddPruneFilter(new ItemCooldownFilter(_cooldownManager)) // Skip items that are on a cooldown.
            .AddPruneFilter(new TransactionAgeFilter(2, 10)) // Skip items that have not been traded in the last X minutes.
            .AddPruneFilter(new Item24HAveragePriceFilter(50, 50_000_000)) // Skip items with a 24-hour average price outside the range X - Y.
            .AddPruneFilter(new TransactionVolumeFilter(2_000_000)) // Skip items with a transaction volume less than X gp.
            .AddPruneFilter(new ReturnOfInvestmentFilter(4)); // Skip items with a return of investment less than X%.
            //.AddPruneFilter(new PotentialProfitFilter(400_000, true));     // Skip items with a potential profit less than X.
            //.AddPruneFilter(new VolatilityFilter(12))                                        // Skip items with a price fluctuation of more than X% in the last 30 minutes.
            
        // Flip filters are used to further filter out items that have passed the prune filters.
        _filterCollection
            .AddFlipFilter(new SpikeRemovalFilter(12))             // Skip items that have spiked in price by more than X% in the last 30 minutes.
            .AddFlipFilter(new PriceDropFilter(10));                                        // Skip items that have not dropped in price by at least X%.
    }
    
    
    /// <summary>
    /// Creates a new Flipper instance.
    /// </summary>
    public static async Task<Flipper> Create(int cooldownMinutes = 5)
    {
        OsrsApiController apiController = new();
        
        ItemMapping mapping = await apiController.GetItemMapping();
        
        ItemCache cache = new(mapping);
        
        Flipper flipper = new Flipper(apiController, cache, cooldownMinutes);
        return flipper;
    }
    
    
    /// <summary>
    /// Finds all potential dumps.
    /// </summary>
    /// <returns></returns>
    public async Task<List<ItemDump>> FindDumps()
    {
        List<ItemDump> flips = new();
        
        _filterCollection.InitializeFilters();
        
        // Loop all available items.
        int itemsPassedPruneCount = 0;
        foreach (CacheEntry entry in _cache.Entries())
        {
            // Safety check: If the pruning filters somehow stop working (or there are none), we won't spam the user with messages and the API with requests.
            if (itemsPassedPruneCount >= 50)
            {
                Logger.Warn("Over 50 items passed all pruning filters." +
                            "This is not generally good, and will result in wasted resources." +
                            "Stopping dump search.");
                break;
            }
            
            // Check if the item passes all pruning filters.
            // DEBUG-TEST: if (entry.Item.Id != 2)
            if (!_filterCollection.PassesPruneTest(entry))
                continue;
            itemsPassedPruneCount++;
            
            // Get the price history for the item.
            ItemPriceHistory? history5Min = await GetPriceHistory(entry.Item, TimeSeriesTimeStep.FiveMinutes);
            if (history5Min == null)
                continue;

            // Check if the item passes all flip filters.
            // DEBUG-TEST: if (entry.Item.Id != 2)
            if (!_filterCollection.PassesFlipTest(entry, history5Min))
                continue;
            
            // Get the additional 6-hour price history for the item.
            ItemPriceHistory? history6Hour = await GetPriceHistory(entry.Item, TimeSeriesTimeStep.SixHours);
            
            ItemDump dump = ConstructDumpObject(entry, history5Min, history6Hour);
            
            // Add the flip to the list and set the item on cooldown.
            flips.Add(dump);
            _cooldownManager.SetCooldown(entry.Item.Id, TimeSpan.FromMinutes(_cooldownMinutes));
        }
        Logger.Verbose($"{itemsPassedPruneCount} items passed all pruning filters.");

        if (DEBUG_FILTERS)
        {
            _filterCollection.DebugFilters();
        }
        
        return flips;
    }


    /// <summary>
    /// Constructs an <see cref="ItemDump"/> from the given <see cref="CacheEntry"/>.
    /// </summary>
    private static ItemDump ConstructDumpObject(CacheEntry entry, ItemPriceHistory history5Min, ItemPriceHistory? history6Hour)
    {
        // The price the item should be bought at to make a profit.
        int priceToBuyAt = entry.PriceLatest.LowestPrice;
        // The price the item should be sold at to make a profit.
        int priceToSellAt = entry.Price1HourAverage.HighestPrice;
        // Calculate the potential profit.
        int margin = priceToSellAt - priceToBuyAt;
        // Calculate the margin with tax.
        margin = (int)(margin * 0.99);
        // Calculate the potential profit.
        int? potentialProfit = entry.Item.HasBuyLimit ? margin * Math.Min(entry.Item.GeBuyLimit, entry.Price24HourAverage.TotalVolume) : null;
        // Calculate the ROI percentage
        double roiPercentage = margin / (double)priceToSellAt * 100;
        
        return new ItemDump(
            entry.Item,
            potentialProfit,
            entry.Price24HourAverage.TotalVolume,
            roiPercentage,
            entry.PriceLatest.BuyPrice,
            entry.PriceLatest.SellPrice,
            entry.PriceLatest.LastBuyTime,
            entry.PriceLatest.LastSellTime,
            entry.Price30MinAverage.AveragePrice,
            entry.Price6HourAverage.AveragePrice,
            history5Min,
            history6Hour);
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
        
        ItemAveragePriceDataCollection? average10MinPrices = await _apiController.Get10MinAveragePrices();
        if (average10MinPrices != null)
            _cache.Update10MinAveragePrices(average10MinPrices);
        else
            Logger.Warn("Failed to load 10 minute average prices");
        
        ItemAveragePriceDataCollection? average30MinPrices = await _apiController.Get30MinAveragePrices();
        if (average30MinPrices != null)
            _cache.Update30MinAveragePrices(average30MinPrices);
        else
            Logger.Warn("Failed to load 30 minute average prices");
        
        ItemAveragePriceDataCollection? average30MinOffsetPrices = await _apiController.Get30MinAveragePrices(Get30MinOffset());
        if (average30MinOffsetPrices != null)
            _cache.Update30MinAverageOffsetPrices(average30MinOffsetPrices);
        else
            Logger.Warn("Failed to load 30 minute average offset prices");
        
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


    private async Task<ItemPriceHistory?> GetPriceHistory(ItemData item, TimeSeriesTimeStep timestep)
    {
        return await _apiController.GetPriceHistory(item, timestep);
    }


    private static DateTime Get5MinOffset()
    {
        // Get the 5-minute period we are currently in, and subtract 5 minutes to get the previous period.
        long currentUnixTime = Utils.DateTimeToUnixTime(DateTime.UtcNow);
        long startOfCurrentPeriod = currentUnixTime / 300;
        long startOfLastPeriod = startOfCurrentPeriod - 1;
        return Utils.UnixTimeToDateTime(startOfLastPeriod * 300);
    }


    private static DateTime Get30MinOffset()
    {
        // Get the 30-minute period we are currently in, and subtract 30 minutes to get the previous period.
        long currentUnixTime = Utils.DateTimeToUnixTime(DateTime.UtcNow);
        long startOfCurrentPeriod = currentUnixTime / 1800;
        long startOfLastPeriod = startOfCurrentPeriod - 1;
        return Utils.UnixTimeToDateTime(startOfLastPeriod * 1800);
    }


    public void Dispose()
    {
        _apiController.Dispose();
    }
}