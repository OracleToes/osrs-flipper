using OsrsFlipper;

namespace ConsoleClient;

/// <summary>
/// Console ui for developing.
/// </summary>
internal static class Program
{
    private const float REFRESH_FLIPPER_DATA_INTERVAL = 20f;


    private static async Task Main(string[] args)
    {
        Logger.Info("Starting...");

        using Flipper flipper = await Flipper.Create(Flipper.Config.Default());

        Logger.Info("Started.");
        Console.Beep();

        while (true)
        {
            // Update the latest data.
            await flipper.RefreshCache();
            
            // Find dumps.
            List<ItemDump> dumps = await flipper.FindDumps();
            
            // Log the dumps.
            if (dumps.Count > 0)
            {
                Console.WriteLine();
                Logger.Info("Dump Detections:");
                foreach (ItemDump dump in dumps)
                    Logger.Info(dump);
                Console.Beep();
            }

            // Wait a bit to keep API maintainers happy :)
            await Task.Delay(TimeSpan.FromSeconds(REFRESH_FLIPPER_DATA_INTERVAL));
        }
    }
}