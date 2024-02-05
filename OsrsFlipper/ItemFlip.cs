using OsrsFlipper.Data.Mapping;

namespace OsrsFlipper;

public class ItemFlip
{
    public readonly ItemData Item;
    public readonly int? PotentialProfit;
    public readonly double RoiPercentage;


    public ItemFlip(ItemData item, int? potentialProfit, double roiPercentage)
    {
        Item = item;
        PotentialProfit = potentialProfit;
        RoiPercentage = roiPercentage;
    }


    public override string ToString()
    {
        string potentialProfit = PotentialProfit.HasValue ? $"{PotentialProfit.Value.SeparateThousands()}gp" : "Unknown";
        return $"{Item.Name} - {potentialProfit} potential profit, {RoiPercentage:F1}% ROI";
    }
}