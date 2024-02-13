using OsrsFlipper.Data.Mapping;

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
    // Calculated values:
    public readonly int? PotentialProfit;
    public readonly int MarginCurrent;
    public readonly int Margin30Min;
    public readonly int Margin6hMin;
    public readonly double RoiPercentage;


    public ItemDump(ItemData item,
        int? potentialProfit,
        int totalVolume24H,
        double roiPercentage,
        int instaBuyPrice,
        int instaSellPrice,
        DateTime lastInstaBuyTime,
        DateTime lastInstaSellTime,
        int averagePrice6Hour,
        int averagePrice30Min)
    {
        Item = item;
        PotentialProfit = potentialProfit;
        TotalVolume24H = totalVolume24H;
        RoiPercentage = roiPercentage;
        InstaBuyPrice = instaBuyPrice;
        InstaSellPrice = instaSellPrice;
        LastInstaBuyTime = lastInstaBuyTime;
        LastInstaSellTime = lastInstaSellTime;
        AveragePrice6Hour = averagePrice6Hour;
        AveragePrice30Min = averagePrice30Min;
    }


    public override string ToString()
    {
        string potentialProfit = PotentialProfit.HasValue ? $"{PotentialProfit.Value.SeparateThousands()}gp" : "Unknown";
        return $"{Item.Name}: {potentialProfit} potential profit. {RoiPercentage:F1}% ROI. (InstaBuy: {InstaBuyPrice.SeparateThousands()}gp, InstaSell: {InstaSellPrice.SeparateThousands()}gp, 6h avg: {AveragePrice6Hour.SeparateThousands()}gp). [Wiki Prices]({Item.OsrsWikiPricesLink})";
    }
}