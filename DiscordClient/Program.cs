using Discord;
using Discord.WebSocket;
using DiscordClient.Configuration;
using OsrsFlipper;

namespace DiscordClient;

/// <summary>
/// Discord client.
/// </summary>
internal static class Program
{
    private static DiscordSocketClient client = null!;
    private static List<SocketTextChannel> channels = null!;
    private static FlipperThread flipperThread = null!;


    private static async Task Main()
    {
        ConfigManager.Initialize();

        // Init the channels we want to receive updates on.
        channels = new List<SocketTextChannel>();
        
        // Init the flipper thread.
        flipperThread = new FlipperThread();
        flipperThread.OnDumpsUpdated += OnDumpsUpdated;
        
        // Init the discord client.
        DiscordSocketConfig config = new()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged & ~GatewayIntents.GuildScheduledEvents & ~GatewayIntents.GuildInvites
        };
        client = new DiscordSocketClient(config);
        client.Log += Log;
        client.Ready += OnBotReady;
        client.MessageReceived += OnMessageReceived;

        // Connect to the discord bot.
        if (string.IsNullOrWhiteSpace(ConfigManager.BotConfig.Token))
        {
            Logger.Error("Please set the bot token in the configuration file.");
            return;
        }
        await client.LoginAsync(TokenType.Bot, ConfigManager.BotConfig.Token);
        await client.StartAsync();
        
        // Register the shutdown event.
        AppDomain.CurrentDomain.ProcessExit += async (_, _) => await OnShutdown();

        // Block the program until it is closed.
        await Task.Delay(-1);
    }


    /// <summary>
    /// Called when the flipper thread has found new dumps.
    /// </summary>
    private static async void OnDumpsUpdated()
    {
        while (flipperThread.Dumps.TryDequeue(out ItemDump? dump))
        {
            try
            {
                Embed embed = DumpEmbedBuilder.BuildEmbed(dump);
                foreach (SocketTextChannel channel in channels)
                    await channel.SendMessageAsync(embed: embed);
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to post a dump update to discord: {e}");
            }
        }
    }


    /// <summary>
    /// Called when a Discord message is received.
    /// </summary>
    private static async Task OnMessageReceived(SocketMessage message)
    {
        if (message.Content == "!ping")
        {
            int ping = client.Latency;
            await message.Channel.SendMessageAsync($"pong! ({ping}ms)");
        }
    }


    private static async Task OnBotReady()
    {
        await client.SetStatusAsync(UserStatus.Online);
        await client.SetGameAsync("Money (That's What I Want)", "https://www.youtube.com/watch?v=t5KU34DrrPI", ActivityType.Listening);

        // Register the channels to post dump updates on.
        foreach (SocketGuild guild in client.Guilds)
        {
            foreach (SocketTextChannel channel in guild.TextChannels)
            {
                if (channel.Name != ConfigManager.BotConfig.DumpTargetChannelName)
                    continue;

                channels.Add(channel);
                await channel.SendMessageAsync(":ok: I'm back online!");
            }
        }

        if (channels.Count == 0)
        {
            Logger.Error($"No channels found with the name '{ConfigManager.BotConfig.DumpTargetChannelName}'.");
            return;
        }

        Logger.Info($"\nFound {channels.Count} channels to post dump updates on.\n");

        flipperThread.Start();
    }
    
    
    private static async Task OnShutdown()
    {
        try
        {
            foreach (SocketTextChannel channel in channels)
            {
                await channel.SendMessageAsync(":tools: Going offline, bye!");
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
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