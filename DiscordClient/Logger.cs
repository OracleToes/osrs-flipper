namespace DiscordClient;

public static class Logger
{
    public static void Warn(object message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Bot@[{DateTime.Now}]: {message}");
        Console.ResetColor();
    }
    
    
    public static void Error(object message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Bot@[{DateTime.Now}]: {message}");
        Console.ResetColor();
    }
    
    
    public static void Info(object message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Bot@[{DateTime.Now}]: {message}");
        Console.ResetColor();
    }
    
    
    public static void Verbose(object message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Bot@[{DateTime.Now}]: {message}");
        Console.ResetColor();
    }
    
    
    public static void Debug(object message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Bot@[{DateTime.Now}]: {message}");
        Console.ResetColor();
    }
}