using OsrsFlipper.Caching;
using OsrsFlipper.Data.TimeSeries;

namespace OsrsFlipper.Filtering.Filters.PruneFilters;

/// <summary>
/// A filter that checks if the latest high price is close enough or below to the average high price over a certain time period.
/// This filter tries to remove most spike detections but allow crashes to be detected.
/// </summary>
internal class SpikeRemovalFilter : FlipFilter
{
    private readonly int _maxHighPriceIncreasePercentage;


    /// <summary>
    /// Constructs a new <see cref="SpikeRemovalFilter"/>.
    /// </summary>
    /// <param name="maxHighPriceIncreasePercentage">How much the latest high price can be above the average high price of the last 30 minutes/6h.</param>
    public SpikeRemovalFilter(int maxHighPriceIncreasePercentage)
    {
        _maxHighPriceIncreasePercentage = maxHighPriceIncreasePercentage;
    }


    protected override bool CanPassFilter(CacheEntry itemData, ItemPriceHistory history5Min)
    {
        // Get the latest high price.
        int latestHighPrice = itemData.PriceLatest.HighestPrice;

        // Check the last 30 minutes (6 * 5min data points).
        for (int i = 0; i < 6; i++)
        {
            // Get the price before the latest price.
            ItemPriceHistoryEntry historyEntry = history5Min.Data[^(2 + i)];
            
            //TODO: Maybe some check for the price being too old?
            
            int? historicalPrice = historyEntry.AvgHighPrice;
            
            if (historicalPrice == null)
                return false;

            // Calculate the percentage increase from the previous price to the latest price.
            double percentageIncreaseFromHistory = ((double)latestHighPrice / historicalPrice.Value - 1) * 100;

            // If the percentage increase is significantly high, consider it a price spike.
            if (percentageIncreaseFromHistory > _maxHighPriceIncreasePercentage)
                return false;
        }

        return true;
    }
}