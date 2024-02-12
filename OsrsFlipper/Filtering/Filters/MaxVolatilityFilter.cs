using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering.Filters;

/// <summary>
/// A filter that checks if the item is too volatile to be considered for flipping.
/// </summary>
internal class MaxVolatilityFilter : FlipFilter
{
    private readonly int _maxVolatilityPercentage;


    /// <summary>
    /// Constructs a new <see cref="MaxVolatilityFilter"/>.
    /// </summary>
    /// <param name="maxVolatilityPercentage">The maximum percentage the average highest price of the last 5 minutes may deviate from the average lowest price of the last 5 minutes.</param>
    public MaxVolatilityFilter(int maxVolatilityPercentage)
    {
        _maxVolatilityPercentage = maxVolatilityPercentage;
    }


    public override bool CheckPass(CacheEntry itemData)
    {
        int margin = itemData.Price5MinAverageOffset.Margin;
        double average = itemData.Price5MinAverageOffset.AveragePrice;
        
        if (average == 0)
            return false; // Avoid division by zero.

        double percentageFluctuation = margin / average * 100.0;
        double priceFluctuationPercentage = Math.Abs(percentageFluctuation); // Ensure a positive percentage.
        
        return priceFluctuationPercentage <= _maxVolatilityPercentage;
    }
}