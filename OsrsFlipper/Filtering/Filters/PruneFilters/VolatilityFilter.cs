using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering.Filters.PruneFilters;

/// <summary>
/// Filters out items that are too volatile (as in their price changes too much).
/// </summary>
internal class VolatilityFilter : PruneFilter
{
    private readonly int _maxVolatilityPercentage;


    /// <summary>
    /// Constructs a new <see cref="VolatilityFilter"/>.
    /// </summary>
    /// <param name="maxVolatilityPercentage">The maximum percentage the averaged highest price may deviate from the average lowest price.</param>
    public VolatilityFilter(int maxVolatilityPercentage)
    {
        _maxVolatilityPercentage = maxVolatilityPercentage;
    }


    protected override bool CanPassFilter(CacheEntry itemData)
    {
        //WARN: This volatility check does not work as intended:
        //WARN: we do not use the true min/max price values of a given time period, but the "averaged" min/max (buy/sell) values over the period.
        int margin = itemData.Price30MinAverageOffset.Margin;
        double average = itemData.Price30MinAverageOffset.AveragePrice;
        
        if (average == 0)
            return false; // Avoid division by zero.

        double percentageFluctuation = margin / average * 100.0;
        double priceFluctuationPercentage = Math.Abs(percentageFluctuation); // Ensure a positive percentage.
        
        return priceFluctuationPercentage <= _maxVolatilityPercentage;
    }
}