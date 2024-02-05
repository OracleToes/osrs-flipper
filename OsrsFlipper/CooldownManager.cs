namespace OsrsFlipper;

internal class CooldownManager
{
    private readonly Dictionary<int, DateTime> _cooldowns = new();
    
    
    public bool IsOnCooldown(int itemId)
    {
        if (_cooldowns.TryGetValue(itemId, out DateTime cooldown))
        {
            bool isOnCooldown = DateTime.UtcNow < cooldown;
            if (!isOnCooldown)
                _cooldowns.Remove(itemId);
            return isOnCooldown;
        }

        return false;
    }
    
    
    public void SetCooldown(int itemId, TimeSpan cooldown)
    {
        _cooldowns[itemId] = DateTime.UtcNow + cooldown;
    }
}