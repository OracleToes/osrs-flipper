namespace DiscordClient;

public static class Logger
{
    public static bool EnableVerboseLogging = true;


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
        if (message.ToString() == string.Empty)
            Console.WriteLine();
        else
            Console.WriteLine($"Bot@[{DateTime.Now}]: {message}");
        Console.ResetColor();
    }


    public static void Verbose(object message)
    {
        if (!EnableVerboseLogging)
            return;
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Bot@[{DateTime.Now}]: {message}");
        Console.ResetColor();
    }


    public static void Debug(object message)
    {
#if DEBUG
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Bot@[{DateTime.Now}]: {message}");
        Console.ResetColor();
#endif
    }
}