using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering.Filters;

/// <summary>
/// A filter that checks if the gp transferred between buyers/sellers in a day using this item is enough.
/// </summary>
internal class TransactionVolumeFilter : FlipFilter
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
        int totalTransactionVolume = itemData.Price24HourAverage.BuyVolume * itemData.Price24HourAverage.BuyPrice +
                                    itemData.Price24HourAverage.SellVolume * itemData.Price24HourAverage.SellPrice;
        return totalTransactionVolume >= _minTransactionVolumePerDay;
    }
}