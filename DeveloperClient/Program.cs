using OsrsFlipper;

namespace DeveloperClient;

/// <summary>
/// Console ui for developing.
/// </summary>
internal static class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Starting flipper test...");
        
        Flipper flipper = new();
        await flipper.Test();
        
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}