using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering;

/// <summary>
/// Represents a filter that an item must pass to be considered for flipping.
/// </summary>
internal abstract class FlipFilter
{
    /// <summary>
    /// Checks if the item passes the filter.
    /// </summary>
    /// <param name="itemData">The item to check.</param>
    /// <returns>True if the item passes the filter, false otherwise.</returns>
    public abstract bool CheckPass(CacheEntry itemData);
}