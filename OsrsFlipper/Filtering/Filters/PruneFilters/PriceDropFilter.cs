using OsrsFlipper.Caching;
using OsrsFlipper.Data.TimeSeries;

namespace OsrsFlipper.Filtering.Filters.PruneFilters;

/// <summary>
/// Only includes items that have just dropped in price.
/// </summary>
internal class PriceDropFilter : FlipFilter
{
    private readonly int _minPriceDropPercentage;


    /// <summary>
    /// Constructs a new <see cref="PriceDropFilter"/>.
    /// </summary>
    /// <param name="minPriceDropPercentage">The minimum price drop percentage (from the 6h median price) that an item must have to pass this filter.</param>
    public PriceDropFilter(int minPriceDropPercentage)
    {
        _minPriceDropPercentage = minPriceDropPercentage;
    }


    protected override bool CanPassFilter(CacheEntry itemData, ItemPriceHistory history5Min)
    {
        // Get the latest low price.
        int latestLowPrice = itemData.PriceLatest.LowestPrice;

        // Get the median low price over the last 3 hours.
        const int hours = 3;
        List<int> lowPricesLast6Hours = history5Min.Data.TakeLast(5 * 12 * hours).Select(entry => entry.AvgLowPrice ?? 0).ToList();
        lowPricesLast6Hours.Sort();
        int medianLowPrice6Hours = lowPricesLast6Hours[lowPricesLast6Hours.Count / 2];

        // Calculate the percentage decrease from the median low price to the latest low price.
        double percentageDecreaseFromMedian = (1 - (double)latestLowPrice / medianLowPrice6Hours) * 100;

        // If the percentage decrease is significantly high, consider it a price drop.
        return percentageDecreaseFromMedian > _minPriceDropPercentage;
    }
}