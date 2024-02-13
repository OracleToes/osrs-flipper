using Discord;
using Discord.WebSocket;
using DiscordClient.Configuration;

namespace DiscordClient;

/// <summary>
/// Discord client.
/// </summary>
internal static class Program
{
    private static DiscordSocketClient client = null!;
    private static List<SocketTextChannel> channels = new();


    private static async Task Main()
    {
        ConfigManager.Initialize();

        client = new DiscordSocketClient();
        client.Log += Log;
        client.Ready += OnBotReady;

        if (string.IsNullOrWhiteSpace(ConfigManager.BotConfig.Token))
        {
            Logger.Error("Please set the bot token in the configuration file.");
            return;
        }
        
        await client.LoginAsync(TokenType.Bot, ConfigManager.BotConfig.Token);
        await client.StartAsync();
        
        // Block the program until it is closed.
        await Task.Delay(-1);
    }


    private static async Task OnBotReady()
    {
        Logger.Info("Ready.\n");
        await client.SetStatusAsync(UserStatus.Online);
        await client.SetGameAsync("Money (That's What I Want)", "https://www.youtube.com/watch?v=t5KU34DrrPI", ActivityType.Listening);

        foreach (SocketGuild guild in client.Guilds)
        {
            foreach (SocketTextChannel channel in guild.TextChannels)
            {
                if (channel.Name != ConfigManager.BotConfig.DumpTargetChannelName)
                    continue;
                
                channels.Add(channel);
                await channel.SendMessageAsync("Hello, world!");
            }
        }
        
        if (channels.Count == 0)
        {
            Logger.Warn($"No channels found with the name '{ConfigManager.BotConfig.DumpTargetChannelName}'.");
            return;
        }
        Logger.Info($"Found {channels.Count} channels to post dump updates on.\n");
    }


    private static Task Log(LogMessage msg)
    {
        switch (msg.Severity)
        {
            case LogSeverity.Critical:
                Logger.Error(msg.ToString());
                break;
            case LogSeverity.Error:
                Logger.Error(msg.ToString());
                break;
            case LogSeverity.Warning:
                Logger.Warn(msg.ToString());
                break;
            case LogSeverity.Info:
                Logger.Info(msg.ToString());
                break;
            case LogSeverity.Verbose:
                Logger.Verbose(msg.ToString());
                break;
            case LogSeverity.Debug:
                Logger.Debug(msg.ToString());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return Task.CompletedTask;
    }
}