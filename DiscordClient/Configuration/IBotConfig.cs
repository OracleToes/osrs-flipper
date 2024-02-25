using Config.Net;

namespace DiscordClient.Configuration;

public interface IBotConfig
{
    [Option(Alias = "token", DefaultValue = "")]
    public string Token { get; set; }
    
    [Option(Alias = "owners", DefaultValue = new[] { "0" } )]
    public string[] Owners { get; set; }
    
    [Option(Alias = "dumps_channel_name", DefaultValue = "dumps")]
    public string DumpTargetChannelName { get; set; }
    
    [Option(Alias = "generated_media_channel_name", DefaultValue = "generated-media")]
    public string GeneratedMediaTargetChannelName { get; set; }
}