using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering.Filters;

/// <summary>
/// A filter that checks if the profit margin is above a certain value.
/// </summary>
internal class ProfitMarginFilter : FlipFilter
{
    private readonly int _minProfitMargin;
    private readonly bool _includeTax;


    /// <summary>
    /// Constructs a new <see cref="ProfitMarginFilter"/>.
    /// </summary>
    /// <param name="minProfitMargin">The minimum profit margin in gp required for an item to pass the filter.</param>
    /// <param name="includeTax">True to include the 1% GE-tax, false otherwise.</param>
    public ProfitMarginFilter(int minProfitMargin, bool includeTax = true)
    {
        _minProfitMargin = minProfitMargin;
        _includeTax = includeTax;
    }


    protected override bool CanPassFilter(CacheEntry itemData)
    {
        // The price the item should be bought at to make a profit.
        int priceToBuyAt = itemData.PriceLatest.LowestPrice;
        // The price the item should be sold at to make a profit.
        int priceToSellAt = itemData.Price1HourAverage.HighestPrice; // NOTE: Possibly use Price1HourAverage.AveragePrice instead of Price1HourAverage.HighestPrice?
        
        // Calculate the margin.
        int margin = priceToSellAt - priceToBuyAt;
        
        // Calculate the margin with tax.
        if (_includeTax)
            margin = (int)(margin * 0.99);
        
        // Check if the margin is above the minimum required.
        return margin >= _minProfitMargin;
    }
}