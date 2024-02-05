using OsrsFlipper;

namespace DeveloperClient;

/// <summary>
/// Console ui for developing.
/// </summary>
internal static class Program
{
    private const float REFRESH_FLIPPER_DATA_INTERVAL = 15f;


    private static async Task Main(string[] args)
    {
        Logger.Info("Starting...");

        Flipper flipper = await Flipper.Create();

        Logger.Info("Started.");
        Console.Beep();

        // Loop until the user presses a key.
        Logger.Info("Press any key to exit...");
        while (!Console.KeyAvailable)
        {
            await flipper.RefreshCache();
            List<ItemFlip> dumps = flipper.FindFlips();
            if (dumps.Count > 0)
            {
                Console.WriteLine();
                Logger.Info("Dumps:");
                foreach (ItemFlip dump in dumps)
                    Logger.Info(dump);
                Console.Beep();
            }

            await Task.Delay(TimeSpan.FromSeconds(REFRESH_FLIPPER_DATA_INTERVAL));
        }

        flipper.Dispose();
    }
}