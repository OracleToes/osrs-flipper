namespace DeveloperClient;

public static class Logger
{
    public static void Warn(object message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"DevClient@[{DateTime.Now}]: {message}");
        Console.ResetColor();
    }
    
    
    public static void Error(object message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"DevClient@[{DateTime.Now}]: {message}");
        Console.ResetColor();
    }
    
    
    public static void Info(object message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"DevClient@[{DateTime.Now}]: {message}");
        Console.ResetColor();
    }
}