using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering.Filters;

/// <summary>
/// A filter that checks if the gp transferred between buyers/sellers in a day using this item is enough.
/// </summary>
internal class TransactionVolumeFilter : PruneFilter
{
    private readonly int _minTransactionVolumePerDay;


    /// <summary>
    /// Constructs a new <see cref="VolumeFilter"/>.
    /// </summary>
    /// <param name="minTransactionVolumePerDay">The minimum amount gp transferred between players per day, for this item to be considered for flipping.</param>
    public TransactionVolumeFilter(int minTransactionVolumePerDay)
    {
        _minTransactionVolumePerDay = minTransactionVolumePerDay;
    }


    protected override bool CanPassFilter(CacheEntry itemData)
    {
        // Experimental: Halve the price's effect to filter out semi-high-value items (20k) with reasonably low volume (1k).
        int buyTransactionVolume = itemData.Price24HourAverage.BuyVolume * (itemData.Price24HourAverage.BuyPrice / 2);
        int sellTransactionVolume = itemData.Price24HourAverage.SellVolume * (itemData.Price24HourAverage.SellPrice / 2);
        
        int totalTransactionVolume = buyTransactionVolume + sellTransactionVolume;
        return totalTransactionVolume >= _minTransactionVolumePerDay;
    }
}