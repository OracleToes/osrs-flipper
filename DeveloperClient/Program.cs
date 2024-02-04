using OsrsFlipper;

namespace DeveloperClient;

/// <summary>
/// Console ui for developing.
/// </summary>
internal static class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        
        Flipper flipper = new();
        await flipper.Test();
        
        Console.ReadKey();
    }
}