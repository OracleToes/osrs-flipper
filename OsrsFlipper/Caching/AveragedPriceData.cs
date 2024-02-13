namespace OsrsFlipper.Caching;

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