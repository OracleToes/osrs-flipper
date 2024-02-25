using Config.Net;

namespace DiscordClient.Configuration;

public interface IFilterConfig
{
    [Option(Alias = "prune_cooldown_minutes", DefaultValue = 8)]
    public int CooldownMinutes { get; set; }
    
    [Option(Alias = "prune_max_transaction_age_low", DefaultValue = 2)]
    public int MaxTransactionAgeLow { get; set; }
    
    [Option(Alias = "prune_max_transaction_age_high", DefaultValue = 8)]
    public int MaxTransactionAgeHigh { get; set; }
    
    [Option(Alias = "prune_average_price_24h_min", DefaultValue = 50)]
    public int AveragePrice24HMin { get; set; }
    
    [Option(Alias = "prune_average_price_24h_max", DefaultValue = 50_000_000)]
    public int AveragePrice24HMax { get; set; }
    
    [Option(Alias = "prune_transaction_volume_min", DefaultValue = 3_000_000)]
    public int TransactionVolumeMin { get; set; }
    
    [Option(Alias = "prune_return_of_investment_min_percentage", DefaultValue = 4)]
    public int RoiMinPercentage { get; set; }
    
    [Option(Alias = "prune_average_volatility_30min_max_percentage", DefaultValue = 12)]
    public int AverageVolatility30MinMaxPercentage { get; set; }
    
    [Option(Alias = "flip_potential_profit_min", DefaultValue = 600_000)]
    public int PotentialProfitMin { get; set; }
    
    [Option(Alias = "flip_potential_profit_include_unknown_limit", DefaultValue = true)]
    public bool PotentialProfitIncludeUnknownLimit { get; set; }
    
    [Option(Alias = "flip_spike_removal_max_high_increase_percentage", DefaultValue = 12)]
    public int MaxHighIncreasePercentage { get; set; }
    
    [Option(Alias = "flip_price_drop_min", DefaultValue = 10)]
    public int PriceDropMin { get; set; }
}