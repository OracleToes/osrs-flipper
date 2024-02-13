using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering.Filters;

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


    protected override bool CanPassFilter(CacheEntry itemData)
    {
        // Get the latest high price.
        int latestHighPrice = itemData.PriceLatest.HighestPrice;

        // Get the average high price over the last 30 minutes.
        int averageHighPrice30Min = itemData.Price30MinAverageOffset.HighestPrice;

        // Get the average high price over the last 6 hours.
        int averageHighPrice6Hours = itemData.Price6HourAverage.LowestPrice;

        // Calculate the percentage increase from the average high price to the latest high price.
        double percentageIncreaseFrom30Min = ((double)latestHighPrice / averageHighPrice30Min - 1) * 100;
        double percentageIncreaseFrom6Hours = ((double)latestHighPrice / averageHighPrice6Hours - 1) * 100;

        // If the percentage increase is greater than _maxHighPriceIncreasePercentage, filter out the item.
        bool under30MinLimit = percentageIncreaseFrom30Min <= _maxHighPriceIncreasePercentage;
        bool under6HoursLimit = percentageIncreaseFrom6Hours <= _maxHighPriceIncreasePercentage;
        return under30MinLimit && under6HoursLimit;
    }
}