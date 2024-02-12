using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering.Filters;

/// <summary>
/// A filter that checks if the item has been traded 
/// </summary>
internal class TradeActivityFilter : FlipFilter
{
    private readonly int _maxVolatilityPercentage;


    /// <summary>
    /// Constructs a new <see cref="TradeActivityFilter"/>.
    /// </summary>
    /// <param name="maxVolatilityPercentage"></param>
    public TradeActivityFilter(int maxVolatilityPercentage)
    {
        
    }


    public override bool CheckPass(CacheEntry itemData)
    {
        
    }
}