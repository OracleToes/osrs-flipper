using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering.Filters;

/// <summary>
/// A filter that checks if the return of investment is above a certain percentage.
/// </summary>
internal class ReturnOfInvestmentFilter : FlipFilter
{
    private readonly int _minRoiPercentage;
    private readonly bool _includeTax;


    /// <summary>
    /// Constructs a new <see cref="ReturnOfInvestmentFilter"/>.
    /// </summary>
    /// <param name="minRoiPercentage">The minimum return of investment percentage required for an item to pass the filter.</param>
    /// <param name="includeTax">True to include the 1% GE-tax (subtract from ROI), false otherwise.</param>
    public ReturnOfInvestmentFilter(int minRoiPercentage, bool includeTax = true)
    {
        _minRoiPercentage = minRoiPercentage;
        _includeTax = includeTax;
    }


    public override bool CheckPass(CacheEntry itemData)
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
        
        // Calculate the return of investment.
        double roi = margin / (double)priceToBuyAt * 100;
        
        // Check if the return of investment is above the minimum required.
        return roi >= _minRoiPercentage;
    }
}