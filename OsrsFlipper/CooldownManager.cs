namespace OsrsFlipper;

/// <summary>
/// Manages cooldowns for certain items, to avoid the same item being detected multiple times in a short period of time.
/// </summary>
internal class CooldownManager
{
    private readonly Dictionary<int, DateTime> _cooldowns = new();
    
    
    /// <summary>
    /// Checks if the item with the given id is currently on cooldown.
    /// </summary>
    /// <param name="itemId">Id of the item to check.</param>
    public bool IsOnCooldown(int itemId)
    {
        if (!_cooldowns.TryGetValue(itemId, out DateTime cooldown))
            return false;
        
        bool isOnCooldown = DateTime.UtcNow < cooldown;
        if (!isOnCooldown)
            _cooldowns.Remove(itemId);
        
        return isOnCooldown;
    }
    
    
    /// <summary>
    /// Sets the item with the given id on cooldown for the given timespan.
    /// </summary>
    /// <param name="itemId">The id of the item to set on cooldown.</param>
    /// <param name="cooldown">For how long the item should be on cooldown.</param>
    public void SetCooldown(int itemId, TimeSpan cooldown)
    {
        _cooldowns[itemId] = DateTime.UtcNow + cooldown;
    }
}