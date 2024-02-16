using OsrsFlipper.Caching;
using OsrsFlipper.Data.TimeSeries;

namespace OsrsFlipper.Filtering;

/// <summary>
/// Represents a filter that determines if the item is a potential flip.
/// <see cref="FlipFilter"/>s have access to the price history of an item.
/// </summary>
internal abstract class FlipFilter
{
    public int ItemsChecked { get; private set; }
    public int ItemsPassed { get; private set; }
    public int ItemsFailed { get; private set; }


    public virtual void Initialize()
    {
        ItemsChecked = 0;
        ItemsPassed = 0;
        ItemsFailed = 0;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="history5Min">Historical price data over a 5-minute interval.</param>
    /// <returns></returns>
    public bool CheckPassFilter(CacheEntry itemData, ItemPriceHistory history5Min)
    {
        ItemsChecked++;
        
        bool passed = CanPassFilter(itemData, history5Min);
        if (passed)
            ItemsPassed++;
        else
            ItemsFailed++;
        
        return passed;
    }


    /// <summary>
    /// Checks if the item passes the filter.
    /// </summary>
    /// <param name="itemData">The item to check.</param>
    /// <param name="history5Min">The price history of the item.</param>
    /// <returns>True if the item passes the filter, false otherwise.</returns>
    protected abstract bool CanPassFilter(CacheEntry itemData, ItemPriceHistory history5Min);
}