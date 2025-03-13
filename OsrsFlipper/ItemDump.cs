using OsrsFlipper.Data.Mapping;
using OsrsFlipper.Data.TimeSeries;

namespace OsrsFlipper;

public class ItemDump
{
    public readonly ItemData Item;
    public readonly int TotalVolume24H;
    public readonly int InstaBuyPrice;
    public readonly int InstaSellPrice;
    public readonly DateTime LastInstaBuyTime;
    public readonly DateTime LastInstaSellTime;
    public readonly int AveragePrice30Min;
    public readonly int AveragePrice6Hour;
    
    /// <summary>
    /// Price history with 5-minute intervals, with up to 365 data points.
    /// </summary>
    public readonly ItemPriceHistory PriceHistory5Min;

    /// <summary>
    /// Price history with 6-hour intervals, with up to 365 data points.
    /// </summary>
    public readonly ItemPriceHistory? PriceHistory6Hour;

    // Calculated values:
    public readonly int? PotentialProfit;
    public readonly double RoiPercentage;

    public readonly int BuyingPrice;


    public ItemDump(ItemData item,
        int? potentialProfit,
        int totalVolume24H,
        double roiPercentage,
        int instaBuyPrice,
        int instaSellPrice,
        DateTime lastInstaBuyTime,
        DateTime lastInstaSellTime,
        int averagePrice30Min,
        int averagePrice6Hour,
        ItemPriceHistory priceHistory5Min,
        ItemPriceHistory? priceHistory6Hour,
        int buyingPrice)
    {
        Item = item;
        PotentialProfit = potentialProfit;
        TotalVolume24H = totalVolume24H;
        RoiPercentage = roiPercentage;
        InstaBuyPrice = instaBuyPrice;
        InstaSellPrice = instaSellPrice;
        LastInstaBuyTime = lastInstaBuyTime;
        LastInstaSellTime = lastInstaSellTime;
        AveragePrice30Min = averagePrice30Min;
        AveragePrice6Hour = averagePrice6Hour;
        PriceHistory5Min = priceHistory5Min;
        PriceHistory6Hour = priceHistory6Hour;
        BuyingPrice = buyingPrice;
    }


    public override string ToString()
    {
        string potentialProfit = PotentialProfit.HasValue ? $"{PotentialProfit.Value.SeparateThousands()} gp" : "Unknown";
        return $"\n\n\n{Item.Name}: {BuyingPrice.SeparateThousands()} gp\npotential profit: {potentialProfit}\nROI: {RoiPercentage:F1}%\nInstaBuy:  {InstaBuyPrice.SeparateThousands()} gp\nInstaSell: {InstaSellPrice.SeparateThousands()} gp\n\n6h avg:    {AveragePrice6Hour.SeparateThousands()} gp)\n24h Volume: {TotalVolume24H.SeparateThousands()}\n\n[Prices]({Item.RuneCapitalLink})";
    }
}
