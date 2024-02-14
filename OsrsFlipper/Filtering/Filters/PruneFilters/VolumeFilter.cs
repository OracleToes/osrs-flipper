using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering.Filters.PruneFilters;

/// <summary>
/// A filter that checks if the item has been traded enough to be considered for flipping.
/// </summary>
internal class VolumeFilter : PruneFilter
{
    private readonly int _minVolumePerDay;


    /// <summary>
    /// Constructs a new <see cref="VolumeFilter"/>.
    /// </summary>
    /// <param name="minVolumePerDay">The minimum amount of trades per day for the item to be considered for flipping.</param>
    public VolumeFilter(int minVolumePerDay)
    {
        _minVolumePerDay = minVolumePerDay;
    }


    protected override bool CanPassFilter(CacheEntry itemData)
    {
        return itemData.Price24HourAverage.TotalVolume >= _minVolumePerDay;
    }
}