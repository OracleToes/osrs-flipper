using System.Reflection;
using AutoUpdateViaGitHubRelease;
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

        CheckForUpdates();

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


    private static void CheckForUpdates()
    {
        Logger.Info("Checking for updates...");
        Assembly assembly = Assembly.GetExecutingAssembly();
        string tempDir = Path.Combine(Path.GetTempPath(), "OsrsFlipper");
        Directory.CreateDirectory(tempDir);
        string updateArchive = Path.Combine(tempDir, "update.zip");
        Version? version = assembly.GetName().Version;
        Task<bool> updateTask = UpdateTools.CheckDownloadNewVersionAsync("japsuu", "osrs-flipper", version, updateArchive);
        bool updateAvailable = updateTask.Result;

        if (!updateAvailable)
            return;

        Console.Write("Update? (Y/N)");
        if (ConsoleKey.Y != Console.ReadKey().Key)
            return;

        string installer = Path.Combine(tempDir, UpdateTools.DownloadExtractInstallerToAsync(tempDir).Result);
        string? destinationDir = Path.GetDirectoryName(assembly.Location);
        UpdateTools.StartInstall(installer, updateArchive, destinationDir); // The application needs to be closed before it can be updated.
        Environment.Exit(0);
    }
}