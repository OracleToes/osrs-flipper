using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering.Filters;

/// <summary>
/// Only includes items that have just dropped in price.
/// </summary>
internal class PriceDropFilter : PruneFilter
{
    private readonly int _minPriceDropPercentage;


    /// <summary>
    /// Constructs a new <see cref="PriceDropFilter"/>.
    /// </summary>
    /// <param name="minPriceDropPercentage">The minimum price drop percentage that an item must have to pass this filter.</param>
    public PriceDropFilter(int minPriceDropPercentage)
    {
        _minPriceDropPercentage = minPriceDropPercentage;
    }


    protected override bool CanPassFilter(CacheEntry itemData)
    {
        //WARN: This volatility check does not work as intended:
        //WARN: we do not use the true min/max price values of a given time period, but the "averaged" min/max (buy/sell) values over the period.
        int margin = itemData.PriceLatest.Margin;
        double average = itemData.Price5MinAverageOffset.AveragePrice;
        
        if (average == 0)
            return false; // Avoid division by zero.

        double percentageFluctuation = margin / average * 100.0;
        
        return Math.Abs(percentageFluctuation) <= _minPriceDropPercentage;

        /*// Check that the lowest component of the latest price has dropped minPriceDropPercentage or more, compared to the last 5 minute average price.
        return itemData.PriceLatest.LowestPrice <= itemData.Price5MinAverageOffset.AveragePrice * (1.0 - _minPriceDropPercentage);*/
    }
}