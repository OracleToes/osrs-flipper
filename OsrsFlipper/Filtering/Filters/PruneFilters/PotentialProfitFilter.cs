using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering.Filters.PruneFilters;

/// <summary>
/// A filter that checks if the potential profit is above a certain value.
/// </summary>
internal class PotentialProfitFilter : PruneFilter
{
    private readonly int _minPotentialProfit;
    private readonly bool _includeUnknownBuyLimit;
    private readonly bool _includeTax;


    /// <summary>
    /// Constructs a new <see cref="PotentialProfitFilter"/>.
    /// </summary>
    /// <param name="minPotentialProfit">The minimum potential profit required for an item to pass the filter.</param>
    /// <param name="includeUnknownBuyLimit">True to include all items with unknown buy limits, false otherwise.</param>
    /// <param name="includeTax">True to include the 1% GE-tax, false otherwise.</param>
    public PotentialProfitFilter(int minPotentialProfit, bool includeUnknownBuyLimit, bool includeTax = true)
    {
        _minPotentialProfit = minPotentialProfit;
        _includeUnknownBuyLimit = includeUnknownBuyLimit;
        _includeTax = includeTax;
    }


    protected override bool CanPassFilter(CacheEntry itemData)
    {
        // If the item has no buy limit and we don't want to include items with unknown buy limits, return false.
        if (!itemData.Item.HasBuyLimit)
            return _includeUnknownBuyLimit;
        
        // The price the item should be bought at to make a profit.
        int priceToBuyAt = itemData.PriceLatest.LowestPrice;
        // The price the item should be sold at to make a profit.
        int priceToSellAt = itemData.Price1HourAverage.HighestPrice; // NOTE: Possibly use Price1HourAverage.AveragePrice instead of Price1HourAverage.HighestPrice?
        
        // Calculate the margin.
        int margin = priceToSellAt - priceToBuyAt;
        
        // Calculate the margin with tax.
        if (_includeTax)
            margin = (int)(margin * 0.99);
        
        // Calculate the potential profit.
        int potentialProfit = margin * Math.Min(itemData.Item.GeBuyLimit, itemData.Price24HourAverage.TotalVolume);
        
        // Check if the potential profit is above the minimum required.
        return potentialProfit >= _minPotentialProfit;
    }
}