using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering.Filters;

/// <summary>
/// Checks if the item has valid price data.
/// If any of the price data is invalid, the item is not considered for flipping.
/// </summary>
internal class ValidDataFilter : FlipFilter
{
    protected override bool CanPassFilter(CacheEntry itemData)
    {
        return itemData.PriceLatest.IsValid &&
               itemData.Price5MinAverageOffset.IsValid &&
               itemData.Price5MinAverage.IsValid &&
               itemData.Price10MinAverage.IsValid &&
               itemData.Price30MinAverage.IsValid &&
               itemData.Price1HourAverage.IsValid &&
               itemData.Price6HourAverage.IsValid &&
               itemData.Price24HourAverage.IsValid;
    }
}