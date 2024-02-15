﻿using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordClient.Configuration;
using DiscordClient.Graphing;
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

    private const string HELP_CMD_MESSAGE = @"
Commands:
- help: Shows this message.
- exit: Exits the bot.
";


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

        // Block the program until it is closed.
        bool shouldExit = false;
        while (!shouldExit)
        {
            string? input = Console.ReadLine();
            if (input == null)
                continue;
            switch (input.ToLower())
            {
                case "help":
                    Logger.Info(HELP_CMD_MESSAGE);
                    break;
                case "exit":
                    await Shutdown();
                    shouldExit = true;
                    break;
                default:
                    Logger.Warn("Unknown command. Type 'help' for a list of commands.");
                    break;
            }
        }
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
                foreach (SocketTextChannel channel in channels)
                {
                    // Get the graph image.
                    MemoryStream memStream = await GraphDrawer.DrawGraph(dump.PriceHistory);
                    FileAttachment graphAttachment = new(memStream, "graph.png");
                    RestUserMessage msg = await channel.SendFileAsync(graphAttachment);
                    string graphUrl = msg.Attachments.First().Url;
                    
                    Embed embed = DumpEmbedBuilder.BuildEmbed(dump, graphUrl);
                    await channel.SendMessageAsync(embed: embed);
                    await msg.DeleteAsync();
                    await Task.Delay(2000);
                }
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

        Logger.Info("");
        Logger.Info($"Found {channels.Count} channels to post dump updates on.\n");
        Logger.Warn("Please do not close this window manually, but use the 'exit' command to close the bot gracefully.");
        Logger.Warn("Type 'help' for a list of commands.");

        flipperThread.Start();
    }
    
    
    private static async Task Shutdown()
    {
        Logger.Info("Shutting down...");
        try
        {
            foreach (SocketTextChannel channel in channels)
            {
                await channel.SendMessageAsync(":tools: Going offline, bye!");
            }
            await client.DisposeAsync();
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