using OsrsFlipper.Data.Mapping;
using OsrsFlipper.Data.Price.Average;
using OsrsFlipper.Data.Price.Latest;

namespace OsrsFlipper.Caching;

public class LatestPriceData
{
    /// <summary>
    /// The item can be instantly bought for this price.
    /// </summary>
    public int BuyPrice;
    
    /// <summary>
    /// The time when the item was last insta-bought for the <see cref="BuyPrice"/>.
    /// </summary>
    public DateTime LastBuyTime;
    
    /// <summary>
    /// The item can be instantly sold for this price.
    /// </summary>
    public int SellPrice;
    
    /// <summary>
    /// The time when the item was last insta-sold for the <see cref="SellPrice"/>.
    /// </summary>
    public DateTime LastSellTime;
    
    /// <summary>
    /// Average price.
    /// </summary>
    public int AveragePrice => (BuyPrice + SellPrice) / 2;
    
    /// <summary>
    /// The time when the item was last bought or sold.
    /// </summary>
    public DateTime LastUpdateTime => LastBuyTime > LastSellTime ? LastBuyTime : LastSellTime;
    
    /// <summary>
    /// Margin without the 1% GE-tax applied.
    /// </summary>
    public int Margin => BuyPrice - SellPrice;
    
    /// <summary>
    /// Margin with the 1% GE-tax applied.
    /// </summary>
    public int MarginWithTax => (int)(Margin * 0.99);
    
    /// <summary>
    /// Returns the lowest price of buy and sell.
    /// </summary>
    public int LowestPrice => BuyPrice < SellPrice ? BuyPrice : SellPrice;
    
    /// <summary>
    /// Returns the highest price of buy and sell.
    /// </summary>
    public int HighestPrice => BuyPrice > SellPrice ? BuyPrice : SellPrice;
    
    public bool IsValid => BuyPrice > 0 && SellPrice > 0;
}

public class AveragedPriceData
{
    /// <summary>
    /// The item can be instantly bought for this price.
    /// </summary>
    public int BuyPrice;
    
    /// <summary>
    /// Items bought over this period.
    /// </summary>
    public int BuyVolume;
    
    /// <summary>
    /// The item can be instantly sold for this price.
    /// </summary>
    public int SellPrice;
    
    /// <summary>
    /// Items sold over this period.
    /// </summary>
    public int SellVolume;
    
    /// <summary>
    /// Average price over the total period.
    /// </summary>
    public int AveragePrice => (BuyPrice + SellPrice) / 2;
    
    /// <summary>
    /// Total volume of items bought and sold.
    /// </summary>
    public int TotalVolume => BuyVolume + SellVolume;
    
    /// <summary>
    /// Margin without the 1% GE-tax applied.
    /// </summary>
    public int Margin => BuyPrice - SellPrice;
    
    /// <summary>
    /// Margin with the 1% GE-tax applied.
    /// </summary>
    public int MarginWithTax => (int)(Margin * 0.99);
    
    /// <summary>
    /// Returns the lowest price of buy and sell.
    /// </summary>
    public int LowestPrice => BuyPrice < SellPrice ? BuyPrice : SellPrice;
    
    /// <summary>
    /// Returns the highest price of buy and sell.
    /// </summary>
    public int HighestPrice => BuyPrice > SellPrice ? BuyPrice : SellPrice;
    
    /// <summary>
    /// If this data entry has enough data to be considered valid.
    /// </summary>
    public bool IsValid => BuyPrice > 0 && SellPrice > 0;
}

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


    /// <summary>
    /// Checks if the item could potentially be flipped.
    /// </summary>
    /// <returns></returns>
    public bool IsPotentiallyFlippable()
    {
        // All items below this price threshold are not worth flipping.
        const int minPriceThreshold = 50;
        
        // If the price has fluctuated more than this threshold in the last hour, it's too volatile to flip.
        const double volatilityThreshold = 15.0;
        
        if (!PriceLatest.IsValid || !Price5MinAverageOffset.IsValid || !Price5MinAverage.IsValid || !Price1HourAverage.IsValid || !Price6HourAverage.IsValid || !Price24HourAverage.IsValid)
            return false; // Not enough data to determine if flippable.
        
        // Check if the item is too cheap to flip.
        if (PriceLatest.HighestPrice < minPriceThreshold)
            return false;
        
        // Check volatility of the hourly average price. Do not take the latest price into account.
        //WARN: This volatility check does not work as intended:
        //WARN: we do not use the true min/max values of a given time period, but the "averaged" min/max values over the period.
        double priceFluctuationPercentage = CalculateFluctuationPercentage(Price5MinAverageOffset.Margin, Price5MinAverageOffset.AveragePrice);
        if (priceFluctuationPercentage > volatilityThreshold)
            return false;
        
        // Check that the item has been traded in the last 2 minutes.
        if (DateTime.UtcNow - PriceLatest.LastUpdateTime > TimeSpan.FromMinutes(2))
            return false;
        
        // The item is flippable.
        return true;
    }


    private static double CalculateFluctuationPercentage(int margin, double average)
    {
        if (average == 0)
            return 0; // Avoid division by zero.

        double percentageFluctuation = margin / average * 100.0;
        return Math.Abs(percentageFluctuation); // Ensure a positive percentage.
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