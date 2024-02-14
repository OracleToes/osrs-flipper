using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering.Filters.PruneFilters;

/// <summary>
/// A filter that checks if an item is currently on cooldown.
/// </summary>
internal class ItemCooldownFilter : PruneFilter
{
    private readonly CooldownManager _cooldownManager;


    public ItemCooldownFilter(CooldownManager cooldownManager)
    {
        _cooldownManager = cooldownManager;
    }


    protected override bool CanPassFilter(CacheEntry itemData)
    {
        return !_cooldownManager.IsOnCooldown(itemData.Item.Id);
    }
}