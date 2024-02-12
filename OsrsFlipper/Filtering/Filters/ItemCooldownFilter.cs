using OsrsFlipper.Caching;

namespace OsrsFlipper.Filtering.Filters;

/// <summary>
/// A filter that checks if an item is currently on cooldown.
/// </summary>
internal class ItemCooldownFilter : FlipFilter
{
    private readonly CooldownManager _cooldownManager;


    public ItemCooldownFilter(CooldownManager cooldownManager)
    {
        _cooldownManager = cooldownManager;
    }


    public override bool CheckPass(CacheEntry itemData)
    {
        return !_cooldownManager.IsOnCooldown(itemData.Item.Id);
    }
}