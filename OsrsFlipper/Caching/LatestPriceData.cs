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
    
    /// <summary>
    /// Returns the transaction time of the lowest price of buy and sell.
    /// </summary>
    public DateTime LowestPriceTime => BuyPrice < SellPrice ? LastBuyTime : LastSellTime;
    
    /// <summary>
    /// Returns the transaction time of the highest price of buy and sell.
    /// </summary>
    public DateTime HighestPriceTime => BuyPrice > SellPrice ? LastSellTime : LastBuyTime;
    
    public bool IsValid => BuyPrice > 0 && SellPrice > 0;
}