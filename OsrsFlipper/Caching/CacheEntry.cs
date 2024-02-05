using OsrsFlipper.Data.Mapping;
using OsrsFlipper.Data.Price.Average;
using OsrsFlipper.Data.Price.Latest;

namespace OsrsFlipper.Caching;

public class CacheEntry
{
    public readonly ItemData Item;
    
    /// <summary>
    /// The item can be instantly bought for this price.
    /// </summary>
    public int InstaBuyPrice;
    
    /// <summary>
    /// The time when the item was last insta-bought for the <see cref="InstaBuyPrice"/>.
    /// </summary>
    public DateTime LastInstaBuyTime = DateTime.MinValue;
    
    /// <summary>
    /// The item can be instantly sold for this price.
    /// </summary>
    public int InstaSellPrice;
    
    /// <summary>
    /// The time when the item was last insta-sold for the <see cref="InstaSellPrice"/>.
    /// </summary>
    public DateTime LastInstaSellTime = DateTime.MinValue;
    
    /// <summary>
    /// <see cref="InstaBuyPrice"/> but averaged over the last 5 minutes.
    /// </summary>
    public int AverageInstaBuyPrice5Min;
    
    /// <summary>
    /// The amount of items instantly bought for the <see cref="InstaBuyPrice"/> in the last 5 minutes.
    /// </summary>
    public int InstaBuyCountLast5Min;
    
    /// <summary>
    /// <see cref="InstaSellPrice"/> but averaged over the last 5 minutes.
    /// </summary>
    public int AverageInstaSellPrice5Min;
    
    /// <summary>
    /// The amount of items instantly sold for the <see cref="InstaSellPrice"/> in the last 5 minutes.
    /// </summary>
    public int InstaSellCountLast5Min;
    
    /// <summary>
    /// <see cref="InstaBuyPrice"/> but averaged over the last 1 hour.
    /// </summary>
    public int AverageInstaBuyPriceLastHour;
    
    /// <summary>
    /// The amount of items instantly bought for the <see cref="InstaBuyPrice"/> in the last 1 hour.
    /// </summary>
    public int InstaBuyCountLastHour;
    
    /// <summary>
    /// <see cref="InstaSellPrice"/> but averaged over the last 1 hour.
    /// </summary>
    public int AverageInstaSellPriceLastHour;
    
    /// <summary>
    /// The amount of items instantly sold for the <see cref="InstaSellPrice"/> in the last 1 hour.
    /// </summary>
    public int InstaSellCountLastHour;
    
    /// <summary>
    /// <see cref="InstaBuyPrice"/> but averaged over the last 24 hours.
    /// </summary>
    public int AverageInstaBuyPriceLast24Hours;
    
    /// <summary>
    /// The amount of items instantly bought for the <see cref="InstaBuyPrice"/> in the last 24 hours.
    /// </summary>
    public int InstaBuyCountLast24Hours;
    
    /// <summary>
    /// <see cref="InstaSellPrice"/> but averaged over the last 24 hours.
    /// </summary>
    public int AverageInstaSellPriceLast24Hours;
    
    /// <summary>
    /// The amount of items instantly sold for the <see cref="InstaSellPrice"/> in the last 24 hours.
    /// </summary>
    public int InstaSellCountLast24Hours;


    public CacheEntry(ItemData item)
    {
        Item = item;
    }


    public void UpdateLatestPrices(ItemLatestPriceData data)
    {
        if (data.HighPrice != null)
            InstaBuyPrice = data.HighPrice.Value;
        LastInstaBuyTime = data.HighTime;

        if (data.LowPrice != null)
            InstaSellPrice = data.LowPrice.Value;
        LastInstaSellTime = data.LowTime;
    }


    public void Update5MinAveragePrices(ItemAveragePriceData data)
    {
        if (data.AverageHighPrice != null)
            AverageInstaBuyPrice5Min = data.AverageHighPrice.Value;
        if (data.HighVolume != null)
            InstaBuyCountLast5Min = data.HighVolume.Value;

        if (data.AverageLowPrice != null)
            AverageInstaSellPrice5Min = data.AverageLowPrice.Value;
        if (data.LowVolume != null)
            InstaSellCountLast5Min = data.LowVolume.Value;
    }


    public void Update1HourAveragePrices(ItemAveragePriceData data)
    {
        if (data.AverageHighPrice != null)
            AverageInstaBuyPriceLastHour = data.AverageHighPrice.Value;
        if (data.HighVolume != null)
            InstaBuyCountLastHour = data.HighVolume.Value;

        if (data.AverageLowPrice != null)
            AverageInstaSellPriceLastHour = data.AverageLowPrice.Value;
        if (data.LowVolume != null)
            InstaSellCountLastHour = data.LowVolume.Value;
    }


    public void Update24HourAveragePrices(ItemAveragePriceData pairValue)
    {
        if (pairValue.AverageHighPrice != null)
            AverageInstaBuyPriceLast24Hours = pairValue.AverageHighPrice.Value;
        if (pairValue.HighVolume != null)
            InstaBuyCountLast24Hours = pairValue.HighVolume.Value;

        if (pairValue.AverageLowPrice != null)
            AverageInstaSellPriceLast24Hours = pairValue.AverageLowPrice.Value;
        if (pairValue.LowVolume != null)
            InstaSellCountLast24Hours = pairValue.LowVolume.Value;
    }
}