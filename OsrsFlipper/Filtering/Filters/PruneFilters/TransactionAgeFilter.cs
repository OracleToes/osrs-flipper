using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering.Filters.PruneFilters;

/// <summary>
/// Filters out items that have too old transaction data.
/// </summary>
internal class TransactionAgeFilter : PruneFilter
{
    private readonly int _maxLowAgeMinutes;
    private readonly int _maxHighAgeMinutes;


    /// <summary>
    /// Constructs a new <see cref="TransactionAgeFilter"/>.
    /// </summary>
    /// <param name="maxLowAgeMinutes">The minimum amount of minutes since the item lows price was last updated, to pass the filter.</param>
    /// <param name="maxHighAgeMinutes">The minimum amount of minutes since the item highs price was last updated, to pass the filter.</param>
    public TransactionAgeFilter(int maxLowAgeMinutes, int maxHighAgeMinutes)
    {
        _maxLowAgeMinutes = maxLowAgeMinutes;
        _maxHighAgeMinutes = maxHighAgeMinutes;
    }


    protected override bool CanPassFilter(CacheEntry itemData)
    {
        bool isLowRecent = DateTime.UtcNow - itemData.PriceLatest.LowestPriceTime <= TimeSpan.FromMinutes(_maxLowAgeMinutes);
        bool isHighRecent = DateTime.UtcNow - itemData.PriceLatest.HighestPriceTime <= TimeSpan.FromMinutes(_maxHighAgeMinutes);
        
        return isLowRecent && isHighRecent;
    }
}