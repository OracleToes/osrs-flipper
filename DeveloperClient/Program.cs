using OsrsFlipper;

namespace DeveloperClient;

/// <summary>
/// Console ui for developing.
/// </summary>
internal static class Program
{
    private const float REFRESH_FLIPPER_DATA_INTERVAL = 20f;


    private static async Task Main(string[] args)
    {
        Logger.Info("Starting...");

        using Flipper flipper = await Flipper.Create();

        Logger.Info("Started.");
        Console.Beep();

        while (true)
        {
            await flipper.RefreshCache();
            List<ItemFlip> dumps = await flipper.FindDumps();
            if (dumps.Count > 0)
            {
                Console.WriteLine();
                Logger.Info("Dump Detections:");
                foreach (ItemFlip dump in dumps)
                    Logger.Info(dump);
                Console.Beep();
            }

            await Task.Delay(TimeSpan.FromSeconds(REFRESH_FLIPPER_DATA_INTERVAL));
        }
    }
}