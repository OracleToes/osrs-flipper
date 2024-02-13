using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering.Filters;

/// <summary>
/// A filter that checks if an item's 24-hour average price is within a certain range.
/// </summary>
internal class Item24HAveragePriceFilter : FlipFilter
{
    private readonly int _minPrice;
    private readonly int _maxPrice;


    /// <summary>
    /// Constructs a new <see cref="Item24HAveragePriceFilter"/>.
    /// </summary>
    /// <param name="minPrice">The minimum price that an item must have to pass the filter. -1 to disable.</param>
    /// <param name="maxPrice">The maximum price that an item must have to pass the filter. -1 to disable.</param>
    public Item24HAveragePriceFilter(int minPrice, int maxPrice)
    {
        _minPrice = minPrice;
        _maxPrice = maxPrice;
    }


    protected override bool CanPassFilter(CacheEntry itemData)
    {
        int averagePrice = itemData.Price24HourAverage.AveragePrice;
        bool tooLow = _minPrice != -1 && averagePrice < _minPrice;
        bool tooHigh = _maxPrice != -1 && averagePrice > _maxPrice;
        
        return !tooLow && !tooHigh;
    }
}