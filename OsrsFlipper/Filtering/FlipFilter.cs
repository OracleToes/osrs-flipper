using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering;

/// <summary>
/// Represents a filter that an item must pass to be considered for flipping.
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


    public bool CheckPassFilter(CacheEntry itemData)
    {
        ItemsChecked++;
        
        bool passed = CanPassFilter(itemData);
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
    /// <returns>True if the item passes the filter, false otherwise.</returns>
    protected abstract bool CanPassFilter(CacheEntry itemData);
}