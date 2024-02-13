using OsrsFlipper.Data.Mapping;
using OsrsFlipper.Data.Price.Average;
using OsrsFlipper.Data.Price.Latest;

namespace OsrsFlipper.Caching;

public class CacheEntry
{
    public readonly ItemData Item;
    
    /// <summary>
    /// Latest price data.
    /// </summary>
    public readonly LatestPriceData PriceLatest = new();
    
    /// <summary>
    /// Pricing data averaged over the last 5 minutes, but not including the 5-minute period we are currently in.
    /// </summary>
    public readonly AveragedPriceData Price5MinAverageOffset = new();
    
    /// <summary>
    /// Pricing data averaged over the last 5 minutes.
    /// </summary>
    public readonly AveragedPriceData Price5MinAverage = new();
    
    /// <summary>
    /// Pricing data averaged over the last 10 minutes.
    /// </summary>
    public readonly AveragedPriceData Price10MinAverage = new();
    
    /// <summary>
    /// Pricing data averaged over the last 30 minutes.
    /// </summary>
    public readonly AveragedPriceData Price30MinAverage = new();
    
    /// <summary>
    /// Pricing data averaged over the last 30 minutes, but not including the 30-minute period we are currently in.
    /// </summary>
    public readonly AveragedPriceData Price30MinAverageOffset = new();
    
    /// <summary>
    /// Pricing data averaged over the last hour.
    /// </summary>
    public readonly AveragedPriceData Price1HourAverage = new();
    
    /// <summary>
    /// Pricing data averaged over the 6 hours.
    /// </summary>
    public readonly AveragedPriceData Price6HourAverage = new();
    
    /// <summary>
    /// Pricing data averaged over the last 24 hours.
    /// </summary>
    public readonly AveragedPriceData Price24HourAverage = new();


    public CacheEntry(ItemData item)
    {
        Item = item;
    }


    public void UpdateLatestPrices(JsonItemLatestPriceData data)
    {
        if (data.HighPrice != null)
        {
            PriceLatest.BuyPrice = data.HighPrice.Value;
            PriceLatest.LastBuyTime = data.HighTime;
        }

        if (data.LowPrice != null)
        {
            PriceLatest.SellPrice = data.LowPrice.Value;
            PriceLatest.LastSellTime = data.LowTime;
        }
    }


    public void Update5MinAverageOffsetPrices(JsonItemAveragePriceData data)
    {
        UpdateAverageData(Price5MinAverageOffset, data);
    }


    public void Update5MinAveragePrices(JsonItemAveragePriceData data)
    {
        UpdateAverageData(Price5MinAverage, data);
    }


    public void Update10MinAveragePrices(JsonItemAveragePriceData data)
    {
        UpdateAverageData(Price10MinAverage, data);
    }


    public void Update30MinAveragePrices(JsonItemAveragePriceData data)
    {
        UpdateAverageData(Price30MinAverage, data);
    }


    public void Update30MinAverageOffsetPrices(JsonItemAveragePriceData data)
    {
        UpdateAverageData(Price30MinAverageOffset, data);
    }


    public void Update1HourAveragePrices(JsonItemAveragePriceData data)
    {
        UpdateAverageData(Price1HourAverage, data);
    }
    
    
    public void Update6HourAveragePrices(JsonItemAveragePriceData data)
    {
        UpdateAverageData(Price6HourAverage, data);
    }


    public void Update24HourAveragePrices(JsonItemAveragePriceData data)
    {
        UpdateAverageData(Price24HourAverage, data);
    }
    
    
    private static void UpdateAverageData(AveragedPriceData data, JsonItemAveragePriceData jsonData)
    {
        if (jsonData.AverageHighPrice != null)
            data.BuyPrice = jsonData.AverageHighPrice.Value;

        if (jsonData.HighVolume != null)
            data.BuyVolume = jsonData.HighVolume.Value;

        if (jsonData.AverageLowPrice != null)
            data.SellPrice = jsonData.AverageLowPrice.Value;

        if (jsonData.LowVolume != null)
            data.SellVolume = jsonData.LowVolume.Value;
    }
}